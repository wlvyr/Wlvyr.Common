/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Data.Common;
using Dapper;

namespace Wlvyr.Common.Data;

/// <summary>
/// Defines a contract for executing SQL commands and queries using a consistent interface,
/// supporting parameterized statements and multi-mapping functionality.
/// </summary>
public interface IDatabaseExecutor
{
    /// <summary>
    /// Executes a non-query SQL command (e.g., INSERT, UPDATE, DELETE).
    /// </summary>
    /// <param name="sql">The SQL command or stored procedure name to execute.</param>
    /// <param name="parameters">Optional parameter set for the command.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteAsync(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns the first result or default if no results are found.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">Optional parameter set for the query.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The first result or default if no result is found.</returns>
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns all results as a sequence of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of result to return.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">Optional parameter set for the query.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A sequence of results.</returns>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query that returns a single result mapped from two types using a split key.
    /// </summary>
    /// <typeparam name="T">The primary type to return.</typeparam>
    /// <typeparam name="TSplit">The secondary type to split and map.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">The parameter set for the query.</param>
    /// <param name="map">A mapping function to combine the two types into one.</param>
    /// <param name="splitOn">The column name on which to split the result set.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The mapped result or default if no result is found.</returns>
    Task<T?> QuerySplitAsync<T, TSplit>(
        string sql,
        DynamicParameters parameters,
        Func<T, TSplit, T> map,
        string splitOn,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query that returns a single result mapped from three types using split keys.
    /// </summary>
    /// <typeparam name="T">The primary type to return.</typeparam>
    /// <typeparam name="TSplit1">The first secondary type.</typeparam>
    /// <typeparam name="TSplit2">The second secondary type.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">The parameter set for the query.</param>
    /// <param name="map">A mapping function to combine the three types into one.</param>
    /// <param name="splitOn">The column name(s) on which to split the result set.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The mapped result or default if no result is found.</returns>
    Task<T?> QuerySplitAsync<T, TSplit1, TSplit2>(
        string sql,
        DynamicParameters parameters,
        Func<T, TSplit1, TSplit2, T> map,
        string splitOn,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query that returns a sequence of results mapped from two types using a split key.
    /// </summary>
    /// <typeparam name="T">The primary type to return.</typeparam>
    /// <typeparam name="TSplit">The secondary type to split and map.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">The parameter set for the query.</param>
    /// <param name="map">A mapping function to combine the two types into one.</param>
    /// <param name="splitOn">The column name on which to split the result set.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A sequence of mapped results.</returns>
    Task<IEnumerable<T>> QuerySplitListAsync<T, TSplit>(
        string sql,
        DynamicParameters parameters,
        Func<T, TSplit, T> map,
        string splitOn,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query that returns a sequence of results mapped from three types using split keys.
    /// </summary>
    /// <typeparam name="T">The primary type to return.</typeparam>
    /// <typeparam name="TSplit1">The first secondary type.</typeparam>
    /// <typeparam name="TSplit2">The second secondary type.</typeparam>
    /// <param name="sql">The SQL query or stored procedure name.</param>
    /// <param name="parameters">The parameter set for the query.</param>
    /// <param name="map">A mapping function to combine the three types into one.</param>
    /// <param name="splitOn">The column name(s) on which to split the result set.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A sequence of mapped results.</returns>
    Task<IEnumerable<T>> QuerySplitListAsync<T, TSplit1, TSplit2>(
        string sql,
        DynamicParameters parameters,
        Func<T, TSplit1, TSplit2, T> map,
        string splitOn,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes a custom asynchronous operation using the underlying <see cref="DbConnection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the operation.</typeparam>
    /// <param name="asyncAction">
    /// A delegate that receives an open <see cref="DbConnection"/> and performs a custom database operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, with the result of type <typeparamref name="T"/>.
    /// </returns>
    /// <remarks>
    /// This method is intended for advanced scenarios where predefined query or command methods are not sufficient,
    /// such as handling custom transactions, bulk operations, or specialized command execution.
    /// The connection is automatically opened and disposed. Transactions must be explicitly managed inside <paramref name="asyncAction"/>.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var result = await executor.ExecuteAsync(async conn =>
    /// {
    ///     using var tx = conn.BeginTransaction();
    ///     var cmd = conn.CreateCommand();
    ///     cmd.CommandText = "DELETE FROM Users WHERE LastLogin &lt; @cutoff";
    ///     cmd.Transaction = tx;
    ///     cmd.Parameters.Add(new SqlParameter("@cutoff", DateTime.UtcNow.AddMonths(-6)));
    ///     await cmd.ExecuteNonQueryAsync();
    ///     tx.Commit();
    ///     return true;
    /// });
    /// </code>
    /// </example>
    Task<T> ExecuteCustomAsync<T>(Func<DbConnection, Task<T>> asyncAction);
}