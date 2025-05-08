/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;
using Wlvyr.Common.Reflection;

namespace Wlvyr.Common.Tests.Reflection;

public class AssemblyExtensionsTests
{
    [Fact]
    public void GetTypesSafely_ReturnsTypesFromAssembly()
    {
        // Arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // Act
        var types = assembly.GetTypesSafely();

        // Assert
        Assert.NotEmpty(types);
        Assert.Contains(types, t => t == typeof(AssemblyExtensionsTests));
    }

    // Note: Testing the exception handling for ReflectionTypeLoadException would require 
    // a test assembly with type loading issues, which is complex to set up in a unit test.
    // In a real-world scenario, you might use a test helper that creates such an assembly,
    // or mock the Assembly.GetTypes() method.
}