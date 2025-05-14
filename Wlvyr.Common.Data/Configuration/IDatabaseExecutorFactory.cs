/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Wlvyr.Common.Data.Configuration;

/// <summary>
/// Defines a factory for creating <see cref="IDatabaseExecutor"/> instances based on the provided context type.
/// </summary>
public interface IDatabaseExecutorFactory
{
    /// <summary>
    /// Creates an <see cref="IDatabaseExecutor"/> for the specified context type.
    /// </summary>
    /// <param name="context">The type representing the database context.</param>
    /// <returns>An instance of <see cref="IDatabaseExecutor"/> configured for the given context.</returns>
    IDatabaseExecutor Create(Type context);
}