/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;
using System.Linq;


namespace Wlvyr.Common.Tests.Configuration;

public class MappingExtensionsTests
{
    [Fact]
    public void CreateMapperConfigs_WhenNoAppSettingsProvided_InitializesOnlywithParameterlessConstructor()
    {
        // Arrange
        var assembly = typeof(TestMapperConfig).Assembly;

        // Act
        var configs = MappingExtensions.CreateMapperConfigs<object>(new[] { assembly });

        // Assert
        // TestMapperConfig, and TestMapperConfigWithOptionalSettings
        Assert.Equal(2,configs.Count());
        Assert.Contains(configs, c => c.GetType() == typeof(TestMapperConfig));
        Assert.Contains(configs, c => c.GetType() == typeof(TestMapperConfigWithOptionalSettings));
        Assert.DoesNotContain(configs, c => c.GetType() == typeof(AbstractMapperConfig));
    }

    [Fact]
    public void CreateMapperConfigs_WhenAppSettingsProvided_UsesAppSettingsConstructor()
    {
        // Arrange
        var assembly = typeof(TestMapperConfigWithSettings).Assembly;
        var appSettings = new DummyAppSettings();

        // Act
        var configs = MappingExtensions.CreateMapperConfigs<object>(new[] { assembly }, appSettings);

        // TestMapperConfig, TestMapperConfigWithSettings, and TestMapperConfigWithOptionalSettings
        Assert.Equal(3,configs.Count());

        // Assert
        var config = configs.Single(c => c.GetType() == typeof(TestMapperConfigWithSettings));

        Assert.IsType<TestMapperConfigWithSettings>(config);
        Assert.Equal(appSettings, ((TestMapperConfigWithSettings)config).ReceivedSettings);
    }

    private class TestMapperConfig : IMapperConfig<object>
    {
        public void Configure(object cfg) { }
    }

    private abstract class AbstractMapperConfig : IMapperConfig<object>
    {
        public abstract void Configure(object cfg);
    }

    private class TestMapperConfigWithSettings : IMapperConfig<object>
    {
        public IAppSettings ReceivedSettings { get; }

        public TestMapperConfigWithSettings(IAppSettings settings)
        {
            ReceivedSettings = settings;
        }

        public void Configure(object cfg) { }
    }

     private class TestMapperConfigWithOptionalSettings : IMapperConfig<object>
    {
        public IAppSettings ReceivedSettings { get; }

        public TestMapperConfigWithOptionalSettings(){ 
            ReceivedSettings = null!;
        }

        public TestMapperConfigWithOptionalSettings(IAppSettings settings)
        {
            ReceivedSettings = settings;
        }

        public void Configure(object cfg) { }
    }

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
}
