/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Data.Common;
using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Data.Configuration;

/// <summary>
/// Represents configuration settings for database connections and executors, 
/// including defaults and type-specific mappings.
/// </summary>
public class DatabaseConfigs
{
    /// <summary>
    /// Gets or sets the name of the default connection string used when no specific mapping is found.
    /// </summary>
#pragma warning disable CS8618
    public string DefaultConnectionName { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// Gets or sets the default factory function used to create <see cref="DbConnection"/> instances.
    /// </summary>
#pragma warning disable CS8618
    public Func<string, DbConnection> DefaultConnectionFactory { get; set; }
#pragma warning restore CS8618

    /// <summary>
    /// Gets or sets the default <see cref="ExecutorKind"/> used when no type-specific mapping is provided.
    /// </summary>
    public ExecutorKind DefaultExecutorKind { get; set; } = ExecutorKind.Default;

    /// <summary>
    /// Gets the mapping of types to specific <see cref="ExecutorKind"/> values.
    /// </summary>
    public Dictionary<Type, ExecutorKind> ExecutorMapping { get; } = new();

    /// <summary>
    /// Gets the mapping of types to specific connection string names.
    /// </summary>
    public Dictionary<Type, string> ConnectionNameMapping { get; } = new();

    /// <summary>
    /// Gets the mapping of types to specific <see cref="DbConnection"/> factory functions.
    /// </summary>
    public Dictionary<Type, Func<string, DbConnection>> ConnectionMapping { get; } = new();
}


/// <summary>
/// Builder class for configuring and constructing a <see cref="DatabaseConfigProvider"/>.
/// </summary>
public class DatabaseConfigProviderBuilder
{
    private readonly DatabaseConfigs _configs = new();
    private readonly IAppSettings _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConfigProviderBuilder"/> class.
    /// </summary>
    /// <param name="configuration">The application settings provider used to retrieve connection strings.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
    public DatabaseConfigProviderBuilder(IAppSettings configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the default connection string name.
    /// </summary>
    /// <param name="name">The name of the default connection string.</param>
    public DatabaseConfigProviderBuilder SetDefaultConnectionName(string name)
    {
        _configs.DefaultConnectionName = name;
        return this;
    }

    /// <summary>
    /// Sets the default connection factory function.
    /// </summary>
    /// <param name="factory">A function that creates a <see cref="DbConnection"/> from a connection string.</param>
    public DatabaseConfigProviderBuilder SetDefaultConnectionFactory(Func<string, DbConnection> factory)
    {
        _configs.DefaultConnectionFactory = factory;
        return this;
    }

    /// <summary>
    /// Sets the default executor kind.
    /// </summary>
    /// <param name="executorKind">The default <see cref="ExecutorKind"/> to use.</param>
    public DatabaseConfigProviderBuilder SetDefaultExecutorKind(ExecutorKind executorKind)
    {
        _configs.DefaultExecutorKind = executorKind;
        return this;
    }

    /// <summary>
    /// Adds a mapping from consumer types to specific executor kinds.
    /// </summary>
    /// <param name="mappings">The type-to-executor-kind mappings.</param>
    public DatabaseConfigProviderBuilder AddExecutorMappings(Dictionary<Type, ExecutorKind> mappings)
    {
        foreach (var kvp in mappings)
            _configs.ExecutorMapping[kvp.Key] = kvp.Value;
        return this;
    }

    /// <summary>
    /// Adds a mapping from consumer types to specific connection string names.
    /// </summary>
    /// <param name="mappings">The type-to-connection-name mappings.</param>
    public DatabaseConfigProviderBuilder AddConnectionNameMappings(Dictionary<Type, string> mappings)
    {
        foreach (var kvp in mappings)
            _configs.ConnectionNameMapping[kvp.Key] = kvp.Value;
        return this;
    }

    /// <summary>
    /// Adds a mapping from consumer types to specific connection factory functions.
    /// </summary>
    /// <param name="mappings">The type-to-connection-factory mappings.</param>
    public DatabaseConfigProviderBuilder AddConnectionFactoryMappings(Dictionary<Type, Func<string, DbConnection>> mappings)
    {
        foreach (var kvp in mappings)
            _configs.ConnectionMapping[kvp.Key] = kvp.Value;
        return this;
    }

    /// <summary>
    /// Builds a <see cref="DatabaseConfigProvider"/> instance using the provided configurations.
    /// </summary>
    /// <returns>A new instance of <see cref="DatabaseConfigProvider"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the required <see cref="DatabaseConfigs.DefaultConnectionName"/> or 
    /// <see cref="DatabaseConfigs.DefaultConnectionFactory"/> are not set.
    /// </exception>
    public DatabaseConfigProvider Build()
    {
        if (string.IsNullOrWhiteSpace(_configs.DefaultConnectionName))
            throw new InvalidOperationException("DefaultConnectionName must be set before building.");

        if (_configs.DefaultConnectionFactory == null)
            throw new InvalidOperationException("DefaultConnectionFactory must be set before building.");

        return new DatabaseConfigProvider(_configuration, _configs);
    }
}
