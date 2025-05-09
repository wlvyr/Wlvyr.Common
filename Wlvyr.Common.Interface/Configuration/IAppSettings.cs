/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


namespace Wlvyr.Common.Interface.Configuration;

/// <summary>
/// Class interface of appsettings.json or app.config or IConfiguration.
/// </summary>
public interface IAppSettings
{
    AppEnvironment Environment { get; init; }
    string GetConnectionString(string name);

    /// <summary>
    /// Get app setting in object form based on T.
    /// </summary>
    /// <param name="key">The identifier of the data to be retrieved</param>
    /// <typeparam name="T">The type of object the data will be converted into</typeparam>
    /// <returns></returns>
    T Get<T>(string key);
}