/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Wlvyr.Common.Data;
using Wlvyr.Common.Data.Configuration;
using Xunit;

namespace Wlvyr.Common.Tests.Data;


public class DatabaseExecutorTests
{
    [Fact]
    public void Constructor_Should_Throw_ArgumentNullException_When_ConnectionString_Is_Null()
    {
        // Arrange
        string connectionString = null!;
        Func<string, DbConnection> connectionFactory = _ => new FakeDbConnection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DatabaseExecutor(connectionString, connectionFactory, ExecutorKind.Default));
    }

    [Fact]
    public void Constructor_Should_Throw_ArgumentNullException_When_ConnectionFactory_Is_Null()
    {
        // Arrange
        string connectionString = "ValidConnectionString";
        Func<string, DbConnection> connectionFactory = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DatabaseExecutor(connectionString, connectionFactory, ExecutorKind.Default));
    }

    [Fact]
    public void Constructor_Should_Not_Throw_When_Arguments_Are_Valid()
    {
        // Arrange
        string connectionString = "ValidConnectionString";
        Func<string, DbConnection> connectionFactory = _ => new FakeDbConnection();

        // Act
        var executor = new DatabaseExecutor(connectionString, connectionFactory, ExecutorKind.Default);

        // Assert
        Assert.NotNull(executor);
    }

    [Fact]
    public async Task ExecuteCustomAsync_Should_Throw_Exception_When_Action_Throws()
    {
        // Arrange
        string connectionString = "Test";
        Func<string, DbConnection> connectionFactory = _ => new FakeDbConnection();
        var executor = new DatabaseExecutor(connectionString, connectionFactory, ExecutorKind.Default);

        Func<DbConnection, Task<int>> throwingAction = _ => throw new InvalidOperationException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteCustomAsync(throwingAction));
    }

    [Fact]
    public async Task ExecuteCustomAsync_Should_Throw_Exception_When_ConnectionFactory_Fails()
    {
        // Arrange
        string connectionString = "TestConnectionString";
        Func<string, DbConnection> connectionFactory = _ => throw new InvalidOperationException("Connection factory failed");

        var executor = new DatabaseExecutor(connectionString, connectionFactory, ExecutorKind.Default);

        // Dummy asyncAction; won't be called
        Func<DbConnection, Task<int>> dummyAction = async conn => await Task.FromResult(1);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => executor.ExecuteCustomAsync(dummyAction));
        Assert.Equal("Connection factory failed", ex.Message);
    }


    // You can use this stub for DbConnection if you don't want to mock
    private class FakeDbConnection : DbConnection
    {
        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel) => null!;
        public override void Close() { }
        public override void ChangeDatabase(string databaseName) { }
        public override void Open() { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override string ConnectionString { get; set; }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public override string Database => "TestDb";
        public override ConnectionState State => ConnectionState.Open;
        public override string DataSource => "FakeSource";
        public override string ServerVersion => "1.0";
        protected override DbCommand CreateDbCommand() => new FakeDbCommand();
    }

    private class FakeDbCommand : DbCommand
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        public override string CommandText { get; set; }
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection? DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection => null!;
        protected override DbTransaction? DbTransaction { get; set; }
        public override void Cancel() { }
        public override int ExecuteNonQuery() => 0;
        public override object ExecuteScalar() => null!;
        public override void Prepare() { }
        protected override DbParameter CreateDbParameter() => null!;
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => null!;
    }
}