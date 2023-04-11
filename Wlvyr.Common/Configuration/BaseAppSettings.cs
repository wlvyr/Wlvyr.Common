using Microsoft.Extensions.Configuration;

using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

public abstract class BaseAppSettings : IAppSettings
{
    public BaseAppSettings(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private readonly IConfiguration configuration;

    public abstract bool IsProductionEnv { get; }

    public abstract string EnvironmentName { get; }

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
            return (T)(object)this.configuration[key];
        }

        var value = this.configuration.GetValue<T>(key);

        if (value != null)
        {
            return value;
        }

        return configuration.GetSection(key).Get<T>();
    }

    public string GetConnectionString(string name)
    {
        return this.configuration.GetConnectionString(name);
    }
}