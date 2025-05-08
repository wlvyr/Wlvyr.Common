/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;

using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

public abstract class BaseAppSettings : IAppSettings
{
    public BaseAppSettings(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.Environment = this.ParseEnvironment() ?? throw new NullReferenceException($"{nameof(Environment)} is null. Cannot resolve DOTNET_ENVIRONMENT or ASPNETCORE_ENVIRONMENT to an {nameof(AppEnvironment)} enum.");
    }

    private readonly IConfiguration configuration;

    public AppEnvironment Environment { get; init; }

    /// <summary>
    /// Get an app setting in object form.
    /// </summary>
    /// <param name="key">Based on .Net Core appsetting.json and IConfiguration: "jsonrootobject:childobject:value" </param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)(this.configuration[key] ?? throw new InvalidOperationException($"Object({nameof(T)}) with key '{key}' not found."));
        }

        var value = this.configuration.GetValue<T>(key);

        if (value != null)
        {
            return value;
        }

        return configuration.GetSection(key).Get<T>() ?? throw new InvalidOperationException($"Object with key '{key}' not found.");
    }

    public string GetConnectionString(string name)
    {
        return this.configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Connection string '{name}' not found.");
    }

    private AppEnvironment? ParseEnvironment()
    {
        var env = configuration["DOTNET_ENVIRONMENT"] ?? configuration["ASPNETCORE_ENVIRONMENT"];

        if (Enum.TryParse<AppEnvironment>(env, true, out var parsed) && Enum.IsDefined(typeof(AppEnvironment), parsed))
        {
            return parsed;
        }

        return null;
    }
}