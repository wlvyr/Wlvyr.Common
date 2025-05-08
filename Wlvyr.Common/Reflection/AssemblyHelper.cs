/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Immutable;
using System.Reflection;

namespace Wlvyr.Common.Reflection;

public static class AssemblyHelper
{
    /// <summary>
    /// Retrieves assemblies loaded in the current <see cref="AppDomain"/>, along with additional assemblies
    /// from the application's base directory whose names match any of the provided substrings.
    /// </summary>
    /// <param name="nameIncludes">
    /// A set of case-insensitive substrings to filter assembly full names. 
    /// If null or empty, all non-dynamic, resolvable assemblies are included.
    /// </param>
    /// <returns>
    /// An enumeration of <see cref="Assembly"/> instances that match the specified filter criteria.
    /// </returns>
    /// <exception cref="BadImageFormatException">
    /// Thrown when a file in the base directory is not a valid .NET assembly (e.g., native DLLs).
    /// </exception>
    /// <exception cref="FileLoadException">
    /// Thrown when an assembly was found but could not be loaded, possibly due to version mismatch or prior load failures.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown when an assembly cannot be found during the load process, typically due to missing dependencies.
    /// </exception>
    /// <exception cref="SecurityException">
    /// Thrown if the caller does not have the required permissions to load the assembly (rare, in sandboxed environments).
    /// </exception>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the file exists but the current user does not have permission to read it.
    /// </exception>
    /// <exception cref="ReflectionTypeLoadException">
    /// Thrown if the assembly is loaded but some of its types could not be loaded due to missing dependencies or type mismatches.
    /// </exception>
    public static IEnumerable<Assembly> GetAssemblies(IReadOnlySet<string> nameIncludes = default!)
    {
        var names = (nameIncludes ?? Enumerable.Empty<string>())
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Select(n => n.ToLowerInvariant())
                        .ToImmutableHashSet();

        // keeping for refrence
        // Another might be Assembly.GetEntryAssembly().GetReferencedAssemblies()
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)) // skip dynamic/in-memory
            .Where(x => !names.Any() ||
                        (!string.IsNullOrWhiteSpace(x.FullName)
                         // better optimized version of x.FullName.ToLowerInvariant().Contains(y) 
                         && names.Any(n =>
                                    x.FullName.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0)
                        )
                  )
            .DistinctBy(x => x.FullName)
            .ToImmutableDictionary(x => x.FullName!, x => x);

        // Load other assemblies from your bin directory
        string path = AppContext.BaseDirectory;
        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            var name = AssemblyName.GetAssemblyName(file);
            if (!names.Any()
                    || (names.Any(n => name.FullName.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0)
                        && !assemblies.ContainsKey(name.FullName))
                )
            {
                assemblies.Add(name.FullName, Assembly.Load(name));
            }
        }

        return assemblies.Values;
    }
}