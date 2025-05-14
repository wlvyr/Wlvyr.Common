/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Data.Common;

namespace Wlvyr.Common.Data.Configuration;

/// <summary>
/// Provides database-related configuration—such as connection strings, connection factories,
/// and execution styles—for a given consumer type. Enables contextual configuration resolution
/// across different layers or components in an application.
/// </summary>
public interface IDatabaseConfigProvider
{
    /// <summary>
    /// Retrieves the database connection string associated with the specified consumer type.
    /// </summary>
    /// <param name="consumerType">The type representing the consuming component or service.</param>
    /// <returns>A valid database connection string.</returns>
    string GetConnectionString(Type consumerType);

    /// <summary>
    /// Retrieves a connection factory delegate that constructs a <see cref="DbConnection"/> 
    /// for the specified consumer type using the resolved connection string.
    /// </summary>
    /// <param name="consumerType">The type representing the consuming component or service.</param>
    /// <returns>A factory function that takes a connection string and returns a <see cref="DbConnection"/>.</returns>
    Func<string, DbConnection> GetConnectionFactory(Type consumerType);

    /// <summary>
    /// Retrieves the execution kind (e.g., text command or stored procedure) to be used 
    /// for the specified consumer type.
    /// </summary>
    /// <param name="consumerType">The type representing the consuming component or service.</param>
    /// <returns>An <see cref="ExecutorKind"/> indicating how commands should be executed.</returns>
    ExecutorKind GetExecutorKind(Type consumerType);
}
