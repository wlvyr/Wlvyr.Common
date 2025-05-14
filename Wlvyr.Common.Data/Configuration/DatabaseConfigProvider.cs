/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Data.Common;
using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Data.Configuration;

/// <inheritdoc />
public class DatabaseConfigProvider : IDatabaseConfigProvider
{
    private readonly DatabaseConfigs _configs;
    private readonly IAppSettings _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseConfigProvider"/> class.
    /// Validates the presence of required defaults in <see cref="DatabaseConfigs"/>.
    /// </summary>
    /// <param name="configuration">The application configuration abstraction.</param>
    /// <param name="configs">The container for database connection mappings and defaults.</param>
    /// <exception cref="ArgumentNullException">Thrown if any required argument is null.</exception>
    /// <exception cref="ArgumentException">Thrown if default connection name or factory is missing.</exception>
    public DatabaseConfigProvider(
        IAppSettings configuration,
        DatabaseConfigs configs
    )
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _configs = configs ?? throw new ArgumentNullException(nameof(configs));

        if (string.IsNullOrWhiteSpace(_configs.DefaultConnectionName))
            throw new ArgumentException("DefaultConnectionName must be provided in DatabaseConfigs.", nameof(configs));

        if (_configs.DefaultConnectionFactory == null)
            throw new ArgumentException("DefaultConnectionFactory must be provided in DatabaseConfigs.", nameof(configs));
    }

    /// <inheritdoc />
    public string GetConnectionString(Type consumerType)
    {
        var name = _configs.ConnectionNameMapping.TryGetValue(consumerType, out var mappedName)
            ? mappedName
            : _configs.DefaultConnectionName;

        var connectionString = _configuration.GetConnectionString(name);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"No connection string found for '{name}'.");

        return connectionString;
    }

    /// <inheritdoc />
    public Func<string, DbConnection> GetConnectionFactory(Type consumerType)
        => _configs.ConnectionMapping.TryGetValue(consumerType, out var factory)
            ? factory
            : _configs.DefaultConnectionFactory;

    /// <inheritdoc />
    public ExecutorKind GetExecutorKind(Type consumerType)
        => _configs.ExecutorMapping.TryGetValue(consumerType, out var kind)
            ? kind
            : _configs.DefaultExecutorKind;
}
