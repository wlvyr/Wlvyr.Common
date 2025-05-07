/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

public static class IAppSettingsExtensions
{
    public static bool IsProduction(this IAppSettings appSettings)
        => appSettings.Environment == AppEnvironment.Production;
    public static bool IsStaging(this IAppSettings appSettings)
        => appSettings.Environment == AppEnvironment.Staging;
    public static bool IsDevelopment(this IAppSettings appSettings)
        => appSettings.Environment == AppEnvironment.Development;
}