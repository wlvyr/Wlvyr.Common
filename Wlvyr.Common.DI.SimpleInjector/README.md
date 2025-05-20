# Wlvyr.Common.DI.SimpleInjector

Wlvyr.Common.DI.SimpleInjector A helper library that simplifies common setup patterns and configurations for the SimpleInjector dependency injection container.

## Usage

### RegisterSingleton Extension Method

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

### SimpleInjectorBootstrapFactory

```cs
using Wlvyr.Common.Configuration;
using Wlvyr.Common.Reflection;
using Wlvyr.Common.DI.SimpleInjector;


var appSettings = new AppSettings(builder.Configuration);

var assemblies = AssemblyHelper.GetAssemblies(
                        nameIncludes: new HashSet<string>() {
                            // assemblies to include
                            // empty for all
                        }
                 );
var excludedIDiConfigFullNames = new HashSet<string>() {
    // IDiConfig to exclude.
};

var diBootstrap = SimpleInjectorBootstrapFactory.CreateBootstrap(
                                assemblies,
                                appSettings,
                                excludedIDiConfigFullNames
                                );
```

## License

This project is under MIT License.