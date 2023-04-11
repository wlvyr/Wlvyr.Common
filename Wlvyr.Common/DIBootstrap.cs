using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Interface.DI;

namespace Wlvyr.Common;

/// <summary>
/// The DIBootstrap helps in initializing your DI library. 
/// </summary>
/// 
/// <typeparam name="TContainer">The DI Container of your chosen DI library</typeparam>
/// <typeparam name="TAppSettings">The object containing appsettings.json (or possibly App.config). </typeparam>
/// 
/// <remark>
/// Implement the abstract class <see cref="Wlvyr.Common.Configuration.BaseAppSettings"/>. 
/// Also, use the <se cref="Microsoft.Extensions.Configuration.ConfigurationBuilder" to build the IConfiguration.
/// </remark>
public class DIBootstrap<TContainer, TAppSettings> : IBootstrap
where TAppSettings : IAppSettings
{
    public DIBootstrap(
        BootstrapConfiguration configuration,
        TAppSettings appSettings,
        Func<TContainer> diContainerFactory,
        Func<IEnumerable<IDIConfig<TContainer, TAppSettings>>> idiConfigsFactory)
    {
        if (configuration == null
        || appSettings == null
        || diContainerFactory == null
        || idiConfigsFactory == null)
        {
            throw new ArgumentNullException($@"
            All arguments of ${nameof(DIBootstrap<TContainer, TAppSettings>)} must not be null.");
        }

        this.Configuration = configuration;
        this.AppSettings = appSettings;
        this.DIContainerFactory = diContainerFactory;
        this.DIContainer = this.DIContainerFactory();
        this.IDIConfigsFactory = idiConfigsFactory;

    }


    /// <summary>
    /// The DI container that will do all the request-instance mapping.
    /// </summary>
    /// <value></value>
    public TContainer DIContainer { get; protected set; }
    /// <summary>
    /// Bootstrap's configuration data.
    /// </summary>
    /// <value></value>
    public BootstrapConfiguration Configuration { get; protected set; }
    /// <summary>
    /// The application's settings
    /// </summary>
    /// <value></value>
    public TAppSettings AppSettings { get; protected set; }
    protected Func<TContainer> DIContainerFactory { get; set; }

    protected Func<IEnumerable<IDIConfig<TContainer, TAppSettings>>> IDIConfigsFactory
    { get; set; }

    /// <summary>
    /// List of actions required before the bootstrap's initalization step.
    /// </summary>
    /// <returns></returns>
    public IList<Action<TContainer>> PreDIInitializationActions { get; } = new List<Action<TContainer>>();
    /// <summary>
    /// List of actions required after the bootstrap's initalization step.
    /// </summary>
    /// <returns></returns>
    public IList<Action<TContainer>> PostDIInitializationActions { get; } = new List<Action<TContainer>>();

    public void Initialize()
    {
        foreach (var action in this.PreDIInitializationActions)
        {
            action(this.DIContainer);
        }

        IEnumerable<IDIConfig<TContainer, TAppSettings>> diconfigs = this.IDIConfigsFactory();
        foreach (var diConfig in diconfigs)
        {
            if (this.Configuration.ExcludedDIConfigFullNames.Contains(diConfig.GetType().FullName))
            {
                continue;
            }

            diConfig.Configure(this.DIContainer, this.AppSettings);
        }

        foreach (var action in this.PostDIInitializationActions)
        {
            action(this.DIContainer);
        }
    }
}