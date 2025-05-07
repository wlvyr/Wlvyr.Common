/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;

using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Configuration;


namespace Wlvyr.Common.Tests.Configuration;

public class BaseAppSettingsTests
{
    [Fact]
    public void Constructor_WithValidConfiguration_SetsEnvironmentCorrectly()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns("Development");

        // Act
        var settings = new TestAppSettings(configMock.Object);

        // Assert
        Assert.Equal(AppEnvironment.Development, settings.Environment);
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestAppSettings(null));
    }

    [Fact]
    public void Constructor_WithInvalidEnvironment_ThrowsNullReferenceException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns("InvalidEnvironment");
        configMock.Setup(c => c["ASPNETCORE_ENVIRONMENT"]).Returns("InvalidEnvironment");

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => new TestAppSettings(configMock.Object));
    }

    [Fact]
    public void Constructor_WithAspNetCoreEnvironment_SetsEnvironmentCorrectly()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns((string)null);
        configMock.Setup(c => c["ASPNETCORE_ENVIRONMENT"]).Returns("Production");

        // Act
        var settings = new TestAppSettings(configMock.Object);

        // Assert
        Assert.Equal(AppEnvironment.Production, settings.Environment);
    }

    [Fact]
    public void Get_WithStringType_ReturnsCorrectValue()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns("Development");
        configMock.Setup(c => c["TestKey"]).Returns("TestValue");
        var settings = new TestAppSettings(configMock.Object);

        // Act
        var result = settings.Get<string>("TestKey");

        // Assert
        Assert.Equal("TestValue", result);
    }

    [Fact]
    public void Get_WithNonExistentStringKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns("Development");
        configMock.Setup(c => c["TestKey"]).Returns((string)null);
        var settings = new TestAppSettings(configMock.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => settings.Get<string>("TestKey"));
    }

    [Fact]
    public void Get_WithNonStringType_UsesGetValue()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        var sectionMock = new Mock<IConfigurationSection>();

        configMock.Setup(c => c["DOTNET_ENVIRONMENT"]).Returns("Development");

        // Setup for GetSection() which is called by GetValue<T>() internally
        configMock.Setup(c => c.GetSection("TestIntKey")).Returns(sectionMock.Object);
        sectionMock.Setup(s => s.Value).Returns("42");

        var settings = new TestAppSettings(configMock.Object);

        // Act
        var result = settings.Get<int>("TestIntKey");

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void Get_WithSectionType_UsesGetSection()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["DOTNET_ENVIRONMENT"] = "Development",
            ["TestSection:Name"] = "Test"
        })
        .Build();

        var settings = new TestAppSettings(configuration);

        // Act
        var result = settings.Get<TestComplexType>("TestSection");

        // Assert
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public void Get_WithNonExistentSectionKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["DOTNET_ENVIRONMENT"] = "Development",
        })
        .Build();

        var settings = new TestAppSettings(configuration);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => settings.Get<TestComplexType>("TestSection"));
    }

    [Fact]
    public void GetConnectionString_WithValidName_ReturnsConnectionString()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
         .AddInMemoryCollection(new Dictionary<string, string>
         {
             ["DOTNET_ENVIRONMENT"] = "Development",
             ["ConnectionStrings:TestDb"] = "Server=localhost;Database=TestDb"
         })
         .Build();

        var settings = new TestAppSettings(configuration);

        // Act
        var result = settings.GetConnectionString("TestDb");

        // Assert
        Assert.Equal("Server=localhost;Database=TestDb", result);
    }

    [Fact]
    public void GetConnectionString_WithNonExistentName_ThrowsInvalidOperationException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            ["DOTNET_ENVIRONMENT"] = "Development",
        })
        .Build();

        var settings = new TestAppSettings(configuration);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => settings.GetConnectionString("TestDb"));
    }

    private class TestAppSettings : BaseAppSettings
    {
        public TestAppSettings(IConfiguration configuration) : base(configuration) { }
    }

    private class TestComplexType
    {
        public string Name { get; set; }
    }
}
