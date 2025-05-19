/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Reflection;
using SimpleInjector;
using Wlvyr.Common.Configuration;
using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.DI.SimpleInjector;

public static class SimpleInjectorBootstrapFactory
{
    /// <summary>
    /// Creates a SimpleInjector bootstrap instance using the specified assemblies and application settings.
    /// </summary>
    /// <typeparam name="TAppSettings">The type of application settings, which must implement <see cref="IAppSettings"/>.</typeparam>
    /// <param name="assemblies">A collection of assemblies from which to load implementations of <see cref="IDIConfig"/>.</param>
    /// <param name="appSettings">The application settings instance used by each <see cref="IDIConfig"/> implementation.</param>
    /// <param name="excludedIDiConfigFullNames">
    /// A set of fully qualified names of <see cref="IDIConfig"/> implementations to exclude. 
    /// For example: "Some.Namespace.ExcludedDIConfig".
    /// </param>
    /// <returns>A <see cref="DIBootstrap{Container, TAppSettings}"/> instance configured with the specified settings and assemblies.</returns>

    public static DIBootstrap<Container, TAppSettings> CreateBootstrap<TAppSettings>(
        IEnumerable<Assembly> assemblies,
        TAppSettings appSettings,
        IReadOnlySet<string> excludedIDiConfigFullNames = default!

    ) where TAppSettings : IAppSettings
    {
        if (assemblies is null)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var diConfig = new DIBootstrapConfiguration<TAppSettings>(
           appSettings,
           excludedIDiConfigFullNames ?? new HashSet<string>()
         );

        var diBootstrap = new DIBootstrap<Container, TAppSettings>(
            diConfig,
            () => new Container(),
            () => assemblies.CreateDIConfigs<Container, TAppSettings>(diConfig.ExcludedDIConfigFullNames)
        );

        return diBootstrap;
    }
}