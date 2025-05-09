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

public class DIBootstrapTests
{
    [Fact]
    public void Constructor_WithNullArguments_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DIBootstrap<object, TestAppSettings>(
            null!,
            () => new object(),
            () => new List<IDIConfig<object, TestAppSettings>>()
        ));

        Assert.Throws<ArgumentNullException>(() => new DIBootstrap<object, TestAppSettings>(
            new DIBootstrapConfiguration<TestAppSettings>(
                new TestAppSettings(),
                new HashSet<string>()),
            null!,
            () => new List<IDIConfig<object, TestAppSettings>>()
        ));

        Assert.Throws<ArgumentNullException>(() => new DIBootstrap<object, TestAppSettings>(
            new DIBootstrapConfiguration<TestAppSettings>(
                new TestAppSettings(),
                new HashSet<string>()),
            () => new object(),
            null!
        ));
    }

    [Fact]
    public void Constructor_WithValidArguments_InitializesDIContainer()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        // Act
        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
        );

        // Assert
        Assert.Same(container, bootstrap.DIContainer);
        Assert.Same(configuration, bootstrap.Configuration);
    }

    [Fact]
    public void Initialize_ExecutesPreInitActions()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
        );

        bool actionCalled = false;
        bootstrap.AddPreDIInitializationActions(c => actionCalled = true);

        // Act
        bootstrap.Initialize();

        // Assert
        Assert.True(actionCalled);
    }

    [Fact]
    public void Initialize_ExecutesPostInitActions()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
        );

        bool actionCalled = false;
        bootstrap.AddPostDIInitializationActions(c => actionCalled = true);

        // Act
        bootstrap.Initialize();

        // Assert
        Assert.True(actionCalled);
    }

    [Fact]
    public void Initialize_ExecutesAllInProperOrder()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        int indexCounter = 0;
        int actualPreInitIndex = -1;
        int actualDIIndex = -1;
        int actualPostInitIndex = -1;

        var diConfig1Mock = new Mock<IDIConfig<object, TestAppSettings>>();
        diConfig1Mock.Setup(x => x.Configure(It.IsAny<object>(), It.IsAny<TestAppSettings>()))
                     .Callback(() =>
                     {
                         actualDIIndex = indexCounter++;
                     });

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
            {
                diConfig1Mock.Object
            }
        );

        bool actionCalled = false;
        bootstrap.AddPreDIInitializationActions(c =>
        {
            actionCalled = true;
            actualPreInitIndex = indexCounter++;
        });

        bootstrap.AddPostDIInitializationActions(c =>
        {
            actionCalled = true;
            actualPostInitIndex = indexCounter++;
        });

        // Act
        bootstrap.Initialize();

        // Assert
        Assert.True(actionCalled);
        Assert.Equal(0, actualPreInitIndex);
        Assert.Equal(1, actualDIIndex);
        Assert.Equal(2, actualPostInitIndex);
    }

    [Fact]
    public void Initialize_CallsConfigureOnAllDIConfigs()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        var diConfig1Mock = new Mock<IDIConfig<object, TestAppSettings>>();
        var diConfig2Mock = new Mock<IDIConfig<object, TestAppSettings>>();

        var diConfigList = new List<IDIConfig<object, TestAppSettings>> { diConfig1Mock.Object, diConfig2Mock.Object };

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => diConfigList
        );

        // Act
        bootstrap.Initialize();

        // Assert
        diConfig1Mock.Verify(c => c.Configure(container, appSettings), Times.Once);
        diConfig2Mock.Verify(c => c.Configure(container, appSettings), Times.Once);
    }

    [Fact]
    public void Initialize_ExcludesDIConfigsInExcludeList()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();

        var diConfig1 = new Config1();
        var diConfig2 = new Config2();

        var excludedNames = new HashSet<string> { "Config1" };

        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            excludedNames
        );

        var diConfigList = new List<IDIConfig<object, TestAppSettings>> { diConfig1, diConfig2 };

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => diConfigList
        );

        // Act
        bootstrap.Initialize();

        // Assert
        Assert.False(diConfig1.WasConfigured);
        Assert.True(diConfig2.WasConfigured);
    }

    [Fact]
    public void AddPreDIInitializationActions_AddsActionsWithoutDuplication()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
        );

        int actionCallCount = 0;
        Action<object> action = c => actionCallCount++;

        // Act
        bootstrap.AddPreDIInitializationActions(action);
        bootstrap.AddPreDIInitializationActions(action); // Adding same action twice
        bootstrap.Initialize();

        // Assert
        Assert.Equal(1, actionCallCount); // Action should only be called once
    }

    [Fact]
    public void AddPostDIInitializationActions_AddsActionsWithoutDuplication()
    {
        // Arrange
        var appSettings = new TestAppSettings();
        var container = new object();
        var configuration = new DIBootstrapConfiguration<TestAppSettings>(
            appSettings,
            new HashSet<string>()
        );

        var bootstrap = new DIBootstrap<object, TestAppSettings>(
            configuration,
            () => container,
            () => new List<IDIConfig<object, TestAppSettings>>()
        );

        int actionCallCount = 0;
        Action<object> action = c => actionCallCount++;

        // Act
        bootstrap.AddPostDIInitializationActions(action);
        bootstrap.AddPostDIInitializationActions(action); // Adding same action twice
        bootstrap.Initialize();

        // Assert
        Assert.Equal(1, actionCallCount); // Action should only be called once
    }

    public class TestAppSettings : IAppSettings
    {
        public AppEnvironment Environment { get; init; } = AppEnvironment.Development;

        public T Get<T>(string key) => default!;

        public string GetConnectionString(string name) => string.Empty;
    }


    public class Config1 : IDIConfig<object, TestAppSettings>
    {
        public bool WasConfigured { get; private set; } = false;
        public void Configure(object container, TestAppSettings appSettings)
        {
            // Do nothing
        }
    }

    public class Config2 : IDIConfig<object, TestAppSettings>
    {
        public bool WasConfigured { get; private set; }

        public void Configure(object container, TestAppSettings appSettings)
        {
            WasConfigured = true;
        }
    }

}

