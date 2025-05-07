/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;
using System;
using Moq;
using System.Collections.Generic;


namespace Wlvyr.Common.Tests.Configuration;

public class MapperBootstrapTests
{
    [Fact]
    public void Constructor_WithNullCfgExpFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MapperBootstrap<object, object>(
            null,
            cfg => new object(),
            () => new List<IMapperConfig<object>>()
        ));
    }

    [Fact]
    public void Constructor_WithNullMapperFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MapperBootstrap<object, object>(
            () => new object(),
            null,
            () => new List<IMapperConfig<object>>()
        ));
    }

    [Fact]
    public void Constructor_WithNullMapperConfigsFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MapperBootstrap<object, object>(
            () => new object(),
            cfg => new object(),
            null
        ));
    }

    [Fact]
    public void Mapper_WhenNotInitialized_ThrowsArgumentNullException()
    {
        // Arrange
        var bootstrap = new MapperBootstrap<object, object>(
            () => new object(),
            cfg => new object(),
            () => new List<IMapperConfig<object>>()
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bootstrap.Mapper);
    }

    [Fact]
    public void Initialize_WithNoMapperConfigs_DoesNotCreateMapper()
    {
        // Arrange
        bool cfgFactoryCalled = false;
        bool mapperFactoryCalled = false;

        var bootstrap = new MapperBootstrap<object, object>(
            () => { cfgFactoryCalled = true; return new object(); },
            cfg => { mapperFactoryCalled = true; return new object(); },
            () => new List<IMapperConfig<object>>() // Empty list
        );

        // Act
        bootstrap.Initialize();

        // Assert
        Assert.False(cfgFactoryCalled);
        Assert.False(mapperFactoryCalled);
        Assert.Throws<ArgumentNullException>(() => bootstrap.Mapper); // Mapper should still be null
    }

    [Fact]
    public void Initialize_WithMapperConfigs_CreatesMapper()
    {
        // Arrange
        var configMock = new Mock<IMapperConfig<object>>();
        var expectedMapper = new object();
        var configurer = new object();

        var bootstrap = new MapperBootstrap<object, object>(
            () => configurer,
            cfg => expectedMapper,
            () => new List<IMapperConfig<object>> { configMock.Object }
        );

        // Act
        bootstrap.Initialize();
        var mapper = bootstrap.Mapper;

        // Assert
        configMock.Verify(c => c.Configure(configurer), Times.Once);
        Assert.Same(expectedMapper, mapper);
    }
}