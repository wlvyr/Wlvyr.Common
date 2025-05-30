# Wlvyr.Common.Data

Wlvyr.Common.Data is a reusable library that provides an ADO.NET-agnostic abstraction for executing database operations. It encapsulates common patterns for interacting with relational databases while remaining decoupled from specific providers (e.g., SQL Server, PostgreSQL, MySQL).

Internally, it uses Dapper, which is a lightweight micro-ORM that works on top of ADO.NET providers. It excels at mapping objects to database command parameters, executing commands, and mapping results back to objects. While it is not a full ORM, it provides an efficient, minimal abstraction layer that allows you to write SQL queries while benefiting from an agnostic approach to the underlying ADO.NET provider.

## Usage

### DatabaseExecutor

Possibly the primary class to be used in this library.

```cs
using Microsoft.Data.SqlClient;
using Wlvyr.Common.Data.Configuration;

// Example DatabaseExecutor to pass in a repository when not expecting stored procedure
var executor = new DatabaseExecutor(
    connectionString:"someconnectionstring",
    connectionFactory: (conn) => new SQLConnection(conn),
    // executorKind - leave blank for default which is text-form sql
);

// Example DatabaseExecutor to pass in a repository when expecting only stored procedure calls
var executor = new DatabaseExecutor(
    connectionString:"someconnectionstring",
    connectionFactory: (conn) => new SQLConnection(conn),
    executorKind: ExecutorKind.StoredProc
);

```

### Using DatabaseExecutor in a Repository

```cs
using Wlvyr.Common.Data;
using Wlvyr.Common.Data.SqlServer;

namespace SomeProject.Data;

// Notice that the repository assumes executor is properly configured for SQL text, and not for stored procedure.
public class SomeRepository(IDatabaseExecutor executor)
{
    public async Task AddSomeAsync(SomeData data)
    {
        var commandText = "INSERT INTO SomeTable (Id, Name) VALUES (@Id, @Name)";

        var parameters = new DynamicParameters();
        parameters.Add("@Id", data.Id, DbType.Int64);
        parameters.Add("@Name", data.Name, DbType.String);

        await executor.ExecuteAsync(commandText, parameters);
    }

    public async Task DoSomethingComplex(SomeData data){

        // Just an example of complex SQL execution with transaction.
        // Not exact code.

        return await executor.ExecuteCustomAsync((connection)=>{

                using var transaction = connection.BeginTransaction();
                
                var sqlInsert = "INSERT INTO SomeTable (Id, Name) VALUES (@Id, @Name)";
                var parameters = new DynamicParameters(new { Id = 123, Name = "Test" });

                // Pass transaction to Dapper methods
                await connection.ExecuteAsync(sqlInsert, parameters, transaction);

                // You can run multiple commands within the same transaction
                await connection.ExecuteAsync("UPDATE OtherTable SET Value = @Value WHERE Id = @Id",
                    new { Value = 42, Id = 5 }, transaction);

                // Commit the transaction after successful commands
                transaction.Commit();
        });
    }
}

// Notice that the repository assumes executor is properly configured for stored procedure calls.
public class SomeStoredProcRepository(IDatabaseExecutor executor)
{
    public async Task AddSomeAsync(SomeData data)
    {
        var storedProcName = "usp_AddSomeData";

        // Use ParameterMetadataProvider for explicit parameter typing
        var parameters = new DynamicParameters();
        parameters.Add("@Id", data.Id, DbType.Int64);
        parameters.Add("@Name", data.Name, DbType.String);

        // set by default is either ParameterMetadataProviderSet or DynamicParameterSet
        // own implementation is allowed by implementing IParameterSet
        await executor.ExecuteAsync(storedProcName, parameters);
    }
}
```

## DatabaseConfigProvider

Provides database-related configuration—such as connection strings, connection factories, and execution styles—for a given consumer type. Enables contextual configuration resolution across different layers or components in an application.

```cs
using Wlvyr.Common.Data.Configuration;
using Microsoft.Data.SqlClient;

namespace SomeProject.Data;

// Example IAppsettings.
var appSettings = new AppSettings(config);

var dbConfigProviderBuilder = new DatabaseConfigProviderBuilder(appSettings);

// Something like this...
dbConfigProviderBuilder
                .SetDefaultConnectionName("DefaultConnectionName")
                .SetDefaultConnectionFactory((conn)=> new SqlConnection(conn))
                .SetDefaultExecutorKind(ExecutorKind.StoredProc)
                
                // Override default ExecutorKind for specific mappings.
                .AddExecutorMapping(new(){
                    {typeof(SomeStoredProcRepository), ExecutorKind.StoredProc }
                })

                // Override default ConnectionName for specific mappings.
                .AddConnectionNameMappings(new() {
                    {typeof(SomeRepository), "CustomConnectionname" }
                })

                // Override default ConnectionFactory for specific mappings.
                .AddConnectionFactoryMappings(new() {
                    // just an example of a repository using a different ADO.NET provider
                    // {typeof(SomeRepository), (conn)=> new PostGressConnection(conn) }
                });

var dbConfigProvider = dbConfigProviderBuilder.Build();
```

The provider can be used in DI to allow correct DatabaseExecutor creation.

In SimpleInjector

```cs
container.RegisterSingleton<IDatabaseConfigProvider>(() => dbConfigProvider);
container.RegisterSingleton<IDatabaseExecutorFactory, DatabaseExecutorFactory>();

// Option 1
//
// Advantage of this approach is it allows the repository
// to have more than one paramater, if needed.
public class SomeRepository {

    protected IDatabaseExecutor _executor;

    public SomeRepository(IDatabaseExecutorFactory factory){
        _executor = factory.Create(this.GetType());
    }
}


// Option 2
//
// Or something more ideal would be using the extension, 
// SimpleInjectorDatabaseExecutorExtensions.RegisterRepository 
// in Wlvyr.Common.DI.SimpleInjector. Only downside with this 
// approach is that only constructors with a single parameter, 
// of IDatabaseExecutor type, can use this.
container.RegisterRepository<ISomeRepository, SomeRepository>();
container.RegisterRepository<ISomRepository2, SomeRepository2>();

```

---

## ADO.NET Providers

Adding for reference.

Please double check if packages are correct before using.

- SQL Server — via `System.Data.SqlClient` or `Microsoft.Data.SqlClient`
- PostgreSQL — via `Npgsql`
- MySQL / MariaDB — via `MySqlConnector` or `MySql.Data`
- SQLite — via `System.Data.SQLite`
- Oracle — via Oracle's `ODP.NET` provider
- Firebird, DB2, etc. — as long as there is an ADO.NET driver

- Should also work on some NoSQL.

### ADO.NET Parameters

IDbDataParameter implementations by ADO.NET provider

| ADO.NET Provider | Parameter Type                                                         |
| ---------------- | ---------------------------------------------------------------------- |
| SQL Server       | `SqlParameter` (`System.Data.SqlClient` or `Microsoft.Data.SqlClient`) |
| PostgreSQL       | `NpgsqlParameter` (`Npgsql`)                                           |
| MySQL            | `MySqlParameter` (`MySqlConnector`)                                    |
| SQLite           | `SQLiteParameter` (`System.Data.SQLite`)                               |
| Oracle           | `OracleParameter` (`Oracle.ManagedDataAccess.Client`)                  |
