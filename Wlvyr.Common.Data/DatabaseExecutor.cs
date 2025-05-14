/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Data;
using System.Data.Common;
using Dapper;
using Wlvyr.Common.Data.Configuration;

namespace Wlvyr.Common.Data;

/// <summary>
/// Provides a unified abstraction for executing SQL commands and queries using Dapper,
/// optionally supporting stored procedures or text-based SQL based on <see cref="ExecutorKind"/>.
/// </summary>
/// <remarks>
/// This class encapsulates the logic to open connections, execute commands,
/// and handle mapping including multi-mapping (splits). It assumes the command type
/// is consistent across operations for the given instance (e.g., all stored proc or all text).
/// </remarks>
public class DatabaseExecutor : IDatabaseExecutor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseExecutor"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string for the target database.</param>
    /// <param name="connectionFactory">A factory delegate to create <see cref="DbConnection"/> instances.</param>
    /// <param name="executorKind">The type of command to execute (e.g., text or stored procedure).</param>
    public DatabaseExecutor(
        string connectionString,
        Func<string, DbConnection> connectionFactory,
        ExecutorKind? executorKind)
    {
        this.ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        this.ConnectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        this.TypeOfCommand = executorKind switch
        {
            ExecutorKind.StoredProc => CommandType.StoredProcedure,
            _ => CommandType.Text
        };
    }

    /// <summary>
    /// The connection string used for database access.
    /// </summary>
    protected string ConnectionString { get; init; }

    /// <summary>
    /// Factory method to generate a <see cref="DbConnection"/> using the configured connection string.
    /// </summary>
    protected Func<string, DbConnection> ConnectionFactory { get; init; }

    /// <summary>
    /// The <see cref="CommandType"/> (Text or StoredProcedure) to be used for commands.
    /// </summary>
    protected CommandType? TypeOfCommand { get; init; }

    /// <inheritdoc />
    public async Task<int> ExecuteAsync(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        // Executes a non-query SQL command (e.g., INSERT, UPDATE, DELETE)
        Func<DbConnection, Task<int>> action = async (conn) =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters ?? new DynamicParameters(),
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return await conn.ExecuteAsync(command);
        };

        return await this.ExecuteCustomAsync(action);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        // Executes a query and returns a sequence of T
        Func<DbConnection, Task<IEnumerable<T>>> action = async (conn) =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters ?? new DynamicParameters(),
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return await conn.QueryAsync<T>(command);
        };

        return await this.ExecuteCustomAsync(action);
    }

    /// <inheritdoc />
    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Func<DbConnection, Task<T>> action = async (conn) =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters ?? new DynamicParameters(),
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

#pragma warning disable CS8603 // Possible null reference return.
            return await conn.QueryFirstOrDefaultAsync<T>(command);
#pragma warning restore CS8603 // Possible null reference return.
        };

        return await this.ExecuteCustomAsync(action);
    }

    /// <inheritdoc />
    public async Task<T?> QuerySplitAsync<T, TSplit>(string sql, DynamicParameters parameters, Func<T, TSplit, T> map, string splitOn, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteCustomAsync(async conn =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return (await conn.QueryAsync<T, TSplit, T>(
                command,
                map,
                splitOn: splitOn
            )).FirstOrDefault();
        });
    }

    /// <inheritdoc />
    public async Task<T?> QuerySplitAsync<T, TSplit1, TSplit2>(string sql, DynamicParameters parameters, Func<T, TSplit1, TSplit2, T> map, string splitOn, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteCustomAsync(async conn =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return (await conn.QueryAsync<T, TSplit1, TSplit2, T>(
                command,
                map,
                splitOn: splitOn
            )).FirstOrDefault();
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> QuerySplitListAsync<T, TSplit>(string sql, DynamicParameters parameters, Func<T, TSplit, T> map, string splitOn, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteCustomAsync(async conn =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return await conn.QueryAsync<T, TSplit, T>(
                command,
                map,
                splitOn: splitOn
            );
        });
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> QuerySplitListAsync<T, TSplit1, TSplit2>(string sql, DynamicParameters parameters, Func<T, TSplit1, TSplit2, T> map, string splitOn, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteCustomAsync(async conn =>
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken,
                commandType: this.TypeOfCommand
            );

            return await conn.QueryAsync<T, TSplit1, TSplit2, T>(
                command,
                map,
                splitOn: splitOn
            );
        });
    }

    /// <summary>
    /// Encapsulates the connection handling logic. Opens the connection and
    /// executes the provided async operation.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="asyncAction">The async function to execute using an open <see cref="DbConnection"/>.</param>
    /// <returns>The result of the async action.</returns>
    public async Task<T> ExecuteCustomAsync<T>(Func<DbConnection, Task<T>> asyncAction)
    {
        try
        {
            using var connection = this.ConnectionFactory(this.ConnectionString);
            await connection.OpenAsync();

            // TODO: log SQL command and parameters before execution if needed

            return await asyncAction(connection);
        }
        catch (Exception)
        {
            // TODO: log exception, including command and parameters
            throw;
        }
    }
}