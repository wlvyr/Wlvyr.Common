namespace Wlvyr.Common.Interface.Configuration;

/// <summary>
/// Class interface of appsettings.json or app.config
/// </summary>
public interface IAppSettings
{
    bool IsProductionEnv { get; }
    string EnvironmentName { get; }
    string GetConnectionString(string name);

    /// <summary>
    /// Get app setting in object form based on T.
    /// </summary>
    /// <param name="key">The identifier of the data to be retrieved</param>
    /// <typeparam name="T">The type of object the data will be converted into</typeparam>
    /// <returns></returns>
    T Get<T>(string key);
}