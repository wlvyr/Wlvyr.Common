/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Wlvyr.Common.Interface.Configuration;
using SimpleInjector;
using Wlvyr.Common.DI.SimpleInjector;

namespace Wlvyr.Common.Tests.Integration.DI;

public class SimpleInjectorBootstrapFactoryTests
{
    private class DummyAppSettings : IAppSettings
    {
        public AppEnvironment Environment { get; init; } = AppEnvironment.Development;

        public T Get<T>(string key)
        {
            return default!;
        }

        public string GetConnectionString(string name)
        {
            return string.Empty;
        }
    }

    private class IncludedDIConfig : IDIConfig<SimpleInjector.Container, DummyAppSettings>
    {
        public void Configure(Container container, DummyAppSettings configuration)
        {
        }
    }

    private class ExcludedDIConfig : IDIConfig<SimpleInjector.Container, DummyAppSettings>
    {
        public void Configure(Container container, DummyAppSettings configuration)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void CreateBootstrap_ShouldThrowArgumentNullException_WhenAssembliesIsNull()
    {
        // Arrange
        var appSettings = new DummyAppSettings();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SimpleInjectorBootstrapFactory.CreateBootstrap(null!, appSettings)
        );

        Assert.Equal("assemblies", ex.ParamName);
    }

    [Fact]
    public void CreateBootstrap_ShouldThrowArgumentNullException_WhenAppSettingsIsNull()
    {
        // Arrange
        var assemblies = new[] { typeof(IncludedDIConfig).Assembly };

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            SimpleInjectorBootstrapFactory.CreateBootstrap<DummyAppSettings>(assemblies, null!)
        );

        Assert.Equal("appSettings", ex.ParamName);
    }


    [Fact]
    public void CreateBootstrap_ShouldReturnDIBootstrap_WithConfiguredComponents()
    {
        // Arrange
        var assemblies = new[] { typeof(IncludedDIConfig).Assembly };
        var appSettings = new DummyAppSettings();
        var excluded = new HashSet<string> { typeof(ExcludedDIConfig).FullName! };

        // Act
        var bootstrap = SimpleInjectorBootstrapFactory.CreateBootstrap(assemblies, appSettings, excluded);

        // Assert
        Assert.NotNull(bootstrap);
        Assert.Equal(appSettings, bootstrap.Configuration.AppSettings);
        Assert.Contains(typeof(ExcludedDIConfig).FullName!, bootstrap.Configuration.ExcludedDIConfigFullNames);
    }

    [Fact]
    public void CreateBootstrap_ShouldDefaultExcludedConfigsToEmptySet_WhenNull()
    {
        // Arrange
        var assemblies = new[] { typeof(IncludedDIConfig).Assembly };
        var appSettings = new DummyAppSettings();

        // Act
        var bootstrap = SimpleInjectorBootstrapFactory.CreateBootstrap(assemblies, appSettings, null!);

        // Assert
        Assert.NotNull(bootstrap);
        Assert.Empty(bootstrap.Configuration.ExcludedDIConfigFullNames);
    }
}