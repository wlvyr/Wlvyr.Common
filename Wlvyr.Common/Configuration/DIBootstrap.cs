/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

/// <summary>
/// The DIBootstrap helps in initializing your DI library. 
/// </summary>
/// 
/// <typeparam name="TContainer">The DI Container of your chosen DI library</typeparam>
/// <typeparam name="TAppSettings">The object containing appsettings.json (or possibly App.config). </typeparam>
/// 
/// <remark>
/// Implement the abstract class <see cref="Wlvyr.Common.Configuration.AppSettings"/>. 
/// Also, use the <se cref="Microsoft.Extensions.Configuration.ConfigurationBuilder" to build the IConfiguration.
/// </remark>
public class DIBootstrap<TContainer, TAppSettings> : IBootstrap
where TAppSettings : IAppSettings
{
    public DIBootstrap(
        DIBootstrapConfiguration<TAppSettings> configuration,
        Func<TContainer> diContainerFactory,
        Func<IEnumerable<IDIConfig<TContainer, TAppSettings>>> idiConfigsFactory)
    {
        if (configuration == null
        || diContainerFactory == null
        || idiConfigsFactory == null)
        {
            throw new ArgumentNullException($@"
            All arguments of ${nameof(DIBootstrap<TContainer, TAppSettings>)} must not be null.");
        }

        this.Configuration = configuration;
        this.IDIConfigsFactory = idiConfigsFactory;
        this.DIContainerFactory = diContainerFactory;

        this.DIContainer = this.DIContainerFactory();
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
    public DIBootstrapConfiguration<TAppSettings> Configuration { get; protected set; }

    protected Func<TContainer> DIContainerFactory { get; set; }

    protected Func<IEnumerable<IDIConfig<TContainer, TAppSettings>>> IDIConfigsFactory
    { get; set; }

    /// <summary>
    /// List of actions required before the bootstrap's initalization step.
    /// </summary>
    /// <returns></returns>
    protected Lazy<IList<Action<TContainer>>> PreDIInitializationActions { get; set; } = new(() => new List<Action<TContainer>>());

    /// <summary>
    /// List of actions required after the bootstrap's initalization step.
    /// </summary>
    /// <returns></returns>
    protected Lazy<IList<Action<TContainer>>> PostDIInitializationActions { get; set; } = new(() => new List<Action<TContainer>>());

    public void Initialize()
    {
        foreach (var action in this.PreDIInitializationActions.Value)
        {
            action(this.DIContainer);
        }

        IEnumerable<IDIConfig<TContainer, TAppSettings>> diconfigs = this.IDIConfigsFactory();
        foreach (var diConfig in diconfigs)
        {
            // Ideally, only IDIConfig that are expected 
            // to be configured are included.
            if (string.IsNullOrWhiteSpace(diConfig?.GetType().FullName) ||
                this.Configuration.ExcludedDIConfigFullNames.Contains(diConfig.GetType().FullName!))
            {
                continue;
            }

            diConfig.Configure(this.DIContainer, this.Configuration.AppSettings);
        }

        foreach (var action in this.PostDIInitializationActions.Value)
        {
            action(this.DIContainer);
        }
    }

    public DIBootstrap<TContainer, TAppSettings> AddPreDIInitializationActions(Action<TContainer> action)
    {
        if (!PreDIInitializationActions.Value.Contains(action))
        {
            PreDIInitializationActions.Value.Add(action);
        }

        return this;
    }

    public DIBootstrap<TContainer, TAppSettings> AddPostDIInitializationActions(Action<TContainer> action)
    {
        if (!PostDIInitializationActions.Value.Contains(action))
        {
            PostDIInitializationActions.Value.Add(action);
        }

        return this;
    }
}