/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Reflection;
using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Reflection;

namespace Wlvyr.Common.Configuration;


public static class DIExtensions
{
    /// <summary>
    /// Creates and returns all <see cref="IDIConfig<TContainer, TAppSettings>"/> found in the provided assemblies.
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TAppSettings"></typeparam>
    /// <param name="assemblies"></param>
    /// <returns>A list of IDIConfig implementation.</returns>
    /// <remarks>
    /// <para>Note: SimpleInjector's TContainer is SimpleInjector.Container.</para>
    /// </remarks>
    public static IEnumerable<IDIConfig<TContainer, TAppSettings>> CreateDIConfigs<TContainer, TAppSettings>(
        this IEnumerable<Assembly> assemblies,
        IReadOnlySet<string> excludedDIConfigFullNames = default!
    ) where TAppSettings : IAppSettings
    {
        var interfaceType = typeof(IDIConfig<TContainer, TAppSettings>);
        var excludedNames = excludedDIConfigFullNames ?? Enumerable.Empty<string>();

        return assemblies.SelectMany(a => a.GetTypesSafely())
                         .Where(t => !t.IsAbstract
                                  && !t.IsInterface
                                  && interfaceType.IsAssignableFrom(t)
                                  && !excludedNames.Contains(t.FullName!)
                                )
                         .Select(t => (IDIConfig<TContainer, TAppSettings>)Activator.CreateInstance(t)!);

        // Keeping for reference.
        // When using SimpleInjector
        // where TContainer : Container
        //    using var tmpDIContainer = containerFactory(); // Func<TContainer>
        //    tmpDIContainer.Register(
        //         typeof(IDIConfig<TContainer, TAppSettings>), 
        //         assemblies
        //    );
        //    IEnumerable<IDIConfig<TContainer, TAppSettings>> diconfigs = tmpDIContainer.GetAllInstances<IDIConfig<TContainer, TAppSettings>>();
    }
}