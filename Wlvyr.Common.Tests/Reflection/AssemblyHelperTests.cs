/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;
using Wlvyr.Common.Reflection;
using System.Collections.Generic;
using System;

namespace Wlvyr.Common.Tests.Reflection;

public class AssemblyHelperTests
{
    [Fact]
    public void GetAssemblies_WithNoIncludes_ReturnsAllAssemblies()
    {
        // Act
        var assemblies = AssemblyHelper.GetAssemblies();

        // Assert
        Assert.NotEmpty(assemblies);
        Assert.Contains(assemblies, a => a == typeof(AssemblyHelper).Assembly);
    }

    [Fact]
    public void GetAssemblies_WithIncludes_ReturnsFilteredAssemblies()
    {
        // Arrange
        var includes = new HashSet<string> { "system.runtime" };

        // Act
        var assemblies = AssemblyHelper.GetAssemblies(includes);

        // Assert
        Assert.NotEmpty(assemblies);
        Assert.Contains(assemblies, a => a.FullName.IndexOf("system.runtime", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    // TODO: Test for: Adds additional assemblies from bin path (integration-like test).
}
