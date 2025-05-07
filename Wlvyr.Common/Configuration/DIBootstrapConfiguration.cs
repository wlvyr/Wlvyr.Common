/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Reflection;
using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

public class DIBootstrapConfiguration<TAppSettings>
        where TAppSettings : IAppSettings
{
    public DIBootstrapConfiguration(
        TAppSettings appSettings,
        IReadOnlySet<string> excludeDIConfigFullNames
    )
    {
        this.AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        this.ExcludedDIConfigFullNames = excludeDIConfigFullNames ?? new HashSet<string>();
    }

    /// <summary>
    /// The application's settings
    /// </summary>
    /// <value></value>
    public TAppSettings AppSettings { get; init; }

    /// <summary>
    /// DI configs to exclude.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    public IReadOnlySet<string> ExcludedDIConfigFullNames { get; init; }
}