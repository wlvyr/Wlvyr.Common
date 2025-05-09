/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;


namespace Wlvyr.Common.Tests.Configuration;

public class DIBootstrapConfigurationTests
{
    [Fact]
    public void Constructor_WithNullAppSettings_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DIBootstrapConfiguration<TestAppSettings>(
            null!,
            new HashSet<string>()
        ));
    }

    [Fact]
    public void Constructor_WithNullExcludedDIConfigFullNames_UsesEmptySet()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var assemblies = new List<Assembly>();

        // Act
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            null!
        );

        // Assert
        Assert.NotNull(configuration.ExcludedDIConfigFullNames);
        Assert.Empty(configuration.ExcludedDIConfigFullNames);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesProperties()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var assemblies = new List<Assembly> { typeof(object).Assembly };
        var excludedNames = new HashSet<string> { "ExcludedConfig" };

        // Act
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            excludedNames
        );

        // Assert
        Assert.Same(appSettings, configuration.AppSettings);
        Assert.Same(excludedNames, configuration.ExcludedDIConfigFullNames);
    }

    private class TestAppSettings : IAppSettings
    {
        public AppEnvironment Environment { get; init; } = AppEnvironment.Development;

        public T Get<T>(string key) => default!;

        public string GetConnectionString(string name) => string.Empty;
    }
}
