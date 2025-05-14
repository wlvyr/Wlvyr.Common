/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Wlvyr.Common.Data.Configuration;

/// <summary>
/// Specifies the type of database executor to use for a given consumer.
/// </summary>
public enum ExecutorKind
{
    /// <summary>
    /// Indicates the default executor should be used.
    /// This typically maps to an implementation such as <c>DatabaseExecutor</c>,
    /// which executes raw SQL queries.
    /// </summary>
    Default,

    /// <summary>
    /// Indicates a stored procedure executor should be used.
    /// This typically maps to an implementation such as <c>StoredProcDatabaseExecutor</c>,
    /// which is optimized for executing stored procedures.
    /// </summary>
    StoredProc
}
