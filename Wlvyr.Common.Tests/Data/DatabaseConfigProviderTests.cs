/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using Wlvyr.Common.Data;
using Wlvyr.Common.Data.Configuration;
using Wlvyr.Common.Interface.Configuration;
using Xunit;

namespace Wlvyr.Common.Tests.Data;

public class DatabaseConfigProviderTests
{
    private class DummyConsumer { }
    private class AnotherConsumer { }

    private readonly Mock<IAppSettings> _mockAppSettings;
    private readonly string _defaultConnName = "DefaultDb";
    private readonly string _anotherConnName = "OtherDb";
    private readonly string _defaultConnStr = "Server=localhost;Database=test;";

    public DatabaseConfigProviderTests()
    {
        _mockAppSettings = new Mock<IAppSettings>();
    }

    private DbConnection FakeConnection(string _) => new FakeDbConnection();

    [Fact]
    public void Build_Throws_When_DefaultConnectionName_NotSet()
    {
        var builder = new DatabaseConfigProviderBuilder(_mockAppSettings.Object);
        builder.SetDefaultConnectionFactory(FakeConnection);

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("DefaultConnectionName must be set before building.", ex.Message);
    }

    [Fact]
    public void Build_Throws_When_DefaultConnectionFactory_NotSet()
    {
        var builder = new DatabaseConfigProviderBuilder(_mockAppSettings.Object);
        builder.SetDefaultConnectionName(_defaultConnName);

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("DefaultConnectionFactory must be set before building.", ex.Message);
    }

    [Fact]
    public void Build_Succeeds_With_Valid_Configuration()
    {
        var builder = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection);

        var provider = builder.Build();
        Assert.NotNull(provider);
    }

    [Fact]
    public void GetConnectionString_Uses_Default_When_No_Mapping()
    {
        _mockAppSettings.Setup(x => x.GetConnectionString(_defaultConnName)).Returns(_defaultConnStr);

        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .Build();

        var connStr = provider.GetConnectionString(typeof(DummyConsumer));
        Assert.Equal(_defaultConnStr, connStr);
    }

    [Fact]
    public void GetConnectionString_Uses_Mapped_Name()
    {
        _mockAppSettings.Setup(x => x.GetConnectionString(_anotherConnName)).Returns("OtherConnStr");

        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .AddConnectionNameMappings(new Dictionary<Type, string>
            {
                { typeof(DummyConsumer), _anotherConnName }
            })
            .Build();

        var connStr = provider.GetConnectionString(typeof(DummyConsumer));
        Assert.Equal("OtherConnStr", connStr);
    }

    [Fact]
    public void GetConnectionString_Throws_When_ConnectionString_NotFound()
    {
        _mockAppSettings.Setup(x => x.GetConnectionString(_defaultConnName)).Returns<string>(null);

        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .Build();

        var ex = Assert.Throws<InvalidOperationException>(() => provider.GetConnectionString(typeof(DummyConsumer)));
        Assert.Contains($"No connection string found for '{_defaultConnName}'", ex.Message);
    }

    [Fact]
    public void GetConnectionFactory_Uses_Default_When_NotMapped()
    {
        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .Build();

        var factory = provider.GetConnectionFactory(typeof(DummyConsumer));
        Assert.NotNull(factory);
        Assert.IsType<FakeDbConnection>(factory.Invoke("any"));
    }

    [Fact]
    public void GetConnectionFactory_Uses_Mapped_Factory()
    {
        Func<string, DbConnection> customFactory = _ => new CustomDbConnection();

        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .AddConnectionFactoryMappings(new Dictionary<Type, Func<string, DbConnection>>
            {
                { typeof(DummyConsumer), customFactory }
            })
            .Build();

        var factory = provider.GetConnectionFactory(typeof(DummyConsumer));
        Assert.IsType<CustomDbConnection>(factory("ignored"));
    }

    [Fact]
    public void GetExecutorKind_Uses_Default_When_NotMapped()
    {
        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .SetDefaultExecutorKind(ExecutorKind.Default)
            .Build();

        Assert.Equal(ExecutorKind.Default, provider.GetExecutorKind(typeof(DummyConsumer)));
    }

    [Fact]
    public void GetExecutorKind_Uses_Mapped_Kind()
    {
        var provider = new DatabaseConfigProviderBuilder(_mockAppSettings.Object)
            .SetDefaultConnectionName(_defaultConnName)
            .SetDefaultConnectionFactory(FakeConnection)
            .SetDefaultExecutorKind(ExecutorKind.Default)
            .AddExecutorMappings(new Dictionary<Type, ExecutorKind>
            {
                { typeof(DummyConsumer), ExecutorKind.StoredProc }
            })
            .Build();

        Assert.Equal(ExecutorKind.StoredProc, provider.GetExecutorKind(typeof(DummyConsumer)));
    }

    [Fact]
    public void Constructor_Throws_If_Null_Args()
    {
        var configs = new DatabaseConfigs
        {
            DefaultConnectionName = _defaultConnName,
            DefaultConnectionFactory = FakeConnection
        };

        Assert.Throws<ArgumentNullException>(() => new DatabaseConfigProvider(null!, configs));
        Assert.Throws<ArgumentNullException>(() => new DatabaseConfigProvider(_mockAppSettings.Object, null!));
    }

    [Fact]
    public void Constructor_Throws_If_Defaults_Missing()
    {
        var configs = new DatabaseConfigs(); // Missing defaults

        var ex = Assert.Throws<ArgumentException>(() => new DatabaseConfigProvider(_mockAppSettings.Object, configs));
        Assert.Contains("DefaultConnectionName", ex.Message);
    }
}

/// <summary>
/// A fake implementation of DbConnection for testing purposes.
/// </summary>
public class FakeDbConnection : DbConnection
{
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override string ConnectionString { get; set; } = string.Empty;
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    public override string Database => "TestDb";
    public override string DataSource => "TestSource";
    public override string ServerVersion => "1.0";
    public override ConnectionState State => ConnectionState.Closed;
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotImplementedException();
    public override void Close() { }
    public override void ChangeDatabase(string databaseName) => throw new NotImplementedException();
    public override void Open() { }
    protected override DbCommand CreateDbCommand() => throw new NotImplementedException();
}

/// <summary>
/// Another fake DbConnection type for mapping test.
/// </summary>
public class CustomDbConnection : FakeDbConnection { }