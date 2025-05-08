/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;


namespace Wlvyr.Common.Tests.Configuration;

public class MappingExtensionsTests
{
    [Fact]
    public void CreateMapperConfigs_FindsAndInstantiatesImplementations()
    {
        // Arrange
        var assembly = typeof(TestMapperConfig).Assembly;

        // Act
        var configs = MappingExtensions.CreateMapperConfigs<object>(new[] { assembly });

        // Assert
        Assert.Contains(configs, c => c.GetType() == typeof(TestMapperConfig));
        Assert.DoesNotContain(configs, c => c.GetType() == typeof(AbstractMapperConfig));
    }

    private class TestMapperConfig : IMapperConfig<object>
    {
        public void Configure(object cfg) { }
    }

    private abstract class AbstractMapperConfig : IMapperConfig<object>
    {
        public abstract void Configure(object cfg);
    }
}
