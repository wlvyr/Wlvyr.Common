/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Wlvyr.Common.Data.Configuration;

/// <summary>
/// A factory implementation that creates <see cref="IDatabaseExecutor"/> instances using configuration data.
/// </summary>
public class DatabaseExecutorFactory : IDatabaseExecutorFactory
{
    private readonly IDatabaseConfigProvider _configProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseExecutorFactory"/> class.
    /// </summary>
    /// <param name="configProvider">The configuration provider used to obtain database execution settings.</param>
    public DatabaseExecutorFactory(IDatabaseConfigProvider configProvider)
    {
        _configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
    }

    /// <summary>
    /// Creates an <see cref="IDatabaseExecutor"/> instance based on the specified context type.
    /// </summary>
    /// <param name="context">The type representing the database context.</param>
    /// <returns>A new <see cref="IDatabaseExecutor"/> configured for the given context.</returns>
    public IDatabaseExecutor Create(Type context)
    {
        var connectionString = _configProvider.GetConnectionString(context);
        var connectionFactory = _configProvider.GetConnectionFactory(context);
        var kind = _configProvider.GetExecutorKind(context);

        return new DatabaseExecutor(connectionString, connectionFactory, kind);
    }
}
