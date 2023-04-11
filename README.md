# Common.NET

Wlvyr.Common is a library created to aid in common project steps.

## Dependency Injection

To help facilitate in loading dependency injection (DI) service across different projects, `Wlvyr.Common.Interface.DI.IDIConfig<TContainer, TSettings>` provides an interface in configuring your DI in all required projects. Then, binding and initializing the DI configurations from `Wlvyr.Common.DIBootstrap<TContainer, TAppSettings>`.

An example in initializing DIBootstrap with SimpleInjector:

```cs
using Microsoft.Extensions.Configuration;
using Wlvyr.Common.SimpleInjectorTools


IEnumerable<Assembly> assemblies; // Pretend you have an IEnumerable of assemblies of all your projects.
var bootstrapConfig = new BootstrapConfiguration(assemblies, "Some_Environment");

var appSettings = new AppSettings(new ConfigurationBuilder().AddJsonFile("appsettings.json") .Build());

var diContainerFactory = () => new SimpleInjector.Container(); // You can use another DI library's container.

var d = new DIBootstrap<SimpleInjector.Container, Configuration.BaseAppSettings>( 
    bootstrapConfig,
    appSettings,
    diContainerFactory,
    () => DIConfigHelper.GetIDIConfigs<SimpleInjector.Container, Configuration.BaseAppSettings>(diContainerFactory, assemblies)
);

d.Initialize();

// d.DIContainer can now be used. example next step: Verify the DI container of your choice that it did bootstrap properly (e.g. SimpleInjector Container has a Verify method.) 

```

Note: BootstrapConfiguration assumes all DI libraries require assemblies to be an input to registering or loading runtime objects. This might not be the case. If so, will have to refactor the class.

## Mapper

Allows project to define their own mapping configuration. The project doesn't require AutoMapper to be used but is based on it. It is basically similar on how DIBootstrap works: Get all `Wlvyr.Common.Interface.Mappers.IMapperConfig<TConfigurer>` to load their configuration then initialize.

Example usage:

```cs

 using Wlvyr.Common.AutoMapperTools;

 public static System.Threading.Tasks.Task Main(string[] args)
    {
        IEnumerable<Assembly> assemblies; // Pretend you have an IEnumerable of assemblies of all your projects.
        var b = new MapperBootstrap<IMapper, MapperConfigurationExpression>(
            MapperConfigHelper.CfgExpFactory, 
            MapperConfigHelper.MapperFactory,
            () => MapperConfigHelper.GetMapperConfigs(assemblies)
        );

        b.Initialize();

        // b.Mapper; can now be accessed. b.Mapper will throw an error if it has not been initialized.
        // b.Mapper can be registered to a DI container. e.g. diBootstrap.DIContainer.RegisterInstance(b.Mapper);
    }
```

## License

This project is under MIT License. Please see [LICENSE](https://github.com/wlvyr/Common.NET/blob/master/LICENSE) for details.
