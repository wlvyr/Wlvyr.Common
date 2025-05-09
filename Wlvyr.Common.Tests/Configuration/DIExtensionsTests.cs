/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;


namespace Wlvyr.Common.Tests.Configuration;

public class DIExtensionsTests
{
    [Fact]
    public void CreateDIConfigs_FindsAndInstantiatesImplementations()
    {
        // Arrange
        var assembly = typeof(TestDIConfig).Assembly;

        // Act
        var configs = DIExtensions.CreateDIConfigs<object, TestAppSettings>(new[] { assembly });

        // Assert
        Assert.Contains(configs, c => c.GetType() == typeof(TestDIConfig));
        Assert.DoesNotContain(configs, c => c.GetType() == typeof(AbstractDIConfig));
    }

    private class TestAppSettings : IAppSettings
    {
        public AppEnvironment Environment { get; init; } = AppEnvironment.Development;

        public T Get<T>(string key) => default!;

        public string GetConnectionString(string name) => string.Empty;
    }

    private class TestDIConfig : IDIConfig<object, TestAppSettings>
    {
        public void Configure(object container, TestAppSettings configuration) { }
    }

    private abstract class AbstractDIConfig : IDIConfig<object, TestAppSettings>
    {
        public abstract void Configure(object container, TestAppSettings configuration);
    }
}

