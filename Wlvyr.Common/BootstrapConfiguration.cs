using System.Reflection;

namespace Wlvyr.Common;

public class BootstrapConfiguration
{
    public BootstrapConfiguration(
        IEnumerable<Assembly> assemblies,
        string environment,
        IEnumerable<string>? excludedDIConfigFullNames = null)
    {
        this.Assemblies = assemblies;
        this.Environment = environment;
        ExcludedDIConfigFullNames = excludedDIConfigFullNames ?? new List<string>();
    }

    /// <summary>
    /// Assemblies containing all the DI configuration.
    /// </summary>
    /// <value></value>
    public IEnumerable<Assembly> Assemblies { get; set; }
    public string Environment { get; set; }
    /// <summary>
    /// DI configs to exclude.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    public IEnumerable<string> ExcludedDIConfigFullNames { get; set; } = new List<string>();
}