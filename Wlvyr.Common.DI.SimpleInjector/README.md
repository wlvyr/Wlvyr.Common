# Wlvyr.Common.DI.SimpleInjector

Wlvyr.Common.DI.SimpleInjector A helper library that simplifies common setup patterns and configurations for the SimpleInjector dependency injection container.

## Usage

```cs
using Wlvyr.Common.DI.SimpleInjector;

var dbConfigProvider = // build your IDatabaseConfigProvider.

// Register  both IDatabaseConfigProvider and DatabaseExecutorFactory
container.RegisterSingleton<IDatabaseConfigProvider>(() => dbConfigProvider);
container.RegisterSingleton<IDatabaseExecutorFactory, DatabaseExecutorFactory>();

// Can use this afterwards.
container.RegisterRepository<ISomeRepository, SomeRepository>();
container.RegisterRepository<ISomRepository2, SomeRepository2>();
```

SomeRepository and SomeRepository2 expect an IDatabaseExecutor as their constructor parameter. Additionally, the extension method only works for repository constructors that have a single parameter of type IDatabaseExecutor.

## License

This project is under MIT License.