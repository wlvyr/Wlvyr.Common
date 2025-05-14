/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Xunit;
using Moq;
using System;
using Wlvyr.Common.Data.Configuration;
using Wlvyr.Common.Data;
using System.Data.Common;

namespace Wlvyr.Common.Tests.Data;

public class DatabaseExecutorFactoryTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConfigProviderIsNull()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new DatabaseExecutorFactory(null!));

        Assert.Equal("configProvider", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldReturnDatabaseExecutor_WithExpectedParameters()
    {
        // Arrange
        var contextType = typeof(DummyContext); // Assume SomeDbContext is a valid type

        var expectedConnectionString = "Server=localhost;Database=Test;";
        Func<string, DbConnection> expectedConnectionFactory = _ => new Mock<DbConnection>().Object;
        var expectedExecutorKind = ExecutorKind.Default; // Assume enum or type

        var mockConfigProvider = new Mock<IDatabaseConfigProvider>();

        mockConfigProvider
            .Setup(p => p.GetConnectionString(contextType))
            .Returns(expectedConnectionString);

        mockConfigProvider
            .Setup(p => p.GetConnectionFactory(contextType))
            .Returns(expectedConnectionFactory);

        mockConfigProvider
            .Setup(p => p.GetExecutorKind(contextType))
            .Returns(expectedExecutorKind);

        var factory = new DatabaseExecutorFactory(mockConfigProvider.Object);

        // Act
        var executor = factory.Create(contextType);

        // Assert
        Assert.NotNull(executor);
        Assert.IsType<DatabaseExecutor>(executor);

        // Optional: cast and verify internals if accessible
        var dbExecutor = executor as DatabaseExecutor;
        Assert.NotNull(dbExecutor);
        // Assuming internals are exposed or testable via properties (if not, reflection may be used)

        mockConfigProvider.Verify(p => p.GetConnectionString(contextType), Times.Once);
        mockConfigProvider.Verify(p => p.GetConnectionFactory(contextType), Times.Once);
        mockConfigProvider.Verify(p => p.GetExecutorKind(contextType), Times.Once);
    }

    private class DummyContext { }
}