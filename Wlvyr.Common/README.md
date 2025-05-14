# Wlvyr.NET

Wlvyr.Common is a library created to aid in common project setup.

## Dependency Injection

To help facilitate in loading dependency injection (DI) service across different projects, `Wlvyr.Common.Interface.Configuration.IDIConfig<TContainer, TSettings>` provides an interface in configuring your DI in all required projects. Then, binding and initializing the DI configurations from `Wlvyr.Common.Configuration.DIBootstrap<TContainer, TAppSettings>`.

An example in initializing DIBootstrap with SimpleInjector:

```cs

using SimpleInjector;

using Wlvyr.Common.Configuration;
using Wlvyr.Common.Reflection;

namespace MainProject;

public static void Main(){

    // Example to get configuration.
    var builder = WebApplication.CreateBuilder(args);
    // Access IConfiguration
    IConfiguration configuration = builder.Configuration; // or new ConfigurationBuilder().AddJsonFile("appsettings.json") .Build()

    IEnumerable<Assembly> assemblies = AssemblyHelper.GetAssemblies(
                                            nameIncludes: new HashSet<string>(){ 
                                                    "asssembly.only-included-project.name1", 
                                                    "assembly.only-included-project.name2" 
                                            }
                                        );

    var diConfig = new  DIBootstrapConfiguration<AppSettings>(
        new AppSettings(configuration),
        new HashSet<string> { "Some.Namespace.ExcludedDIConfig" } // excluded IDIconfig from being configured. empty for all to be included
    );

    var diBootstrap = new DIBootstrap(
        diConfig,
        () => new SimpleInjector.Container(), 
        () => assemblies.CreateDIConfigs<Container, TAppSettings>(diConfig.ExcludedDIConfigFullNames) // from DIExtensions.CreateDIConfigs<Container, AppSettings>
    );

    diBootstrap.Initialize();

    // d.DIContainer can now be used. example next step: Verify the DI container of your choice that it did bootstrap properly. (DIContainer.Verify is from SimpleInjector)
    d.DIContainer.Verify();   
}

```

Then an implementing IDIConfig will look like

```cs

// In Some project that doesn't have to be in the same project as DIBootstrap init nor the main application project.
using SimpleInjector;

using Wlvyr.Common.Interface.Configuration;

namespace SomeProject;

public class SomeDIConfig : IDIConfig<Container, SomeAppSetting> {

    public void Configure(Container container, SomeAppSetting configuration){
        container.Register<ILogger, FileLogger>();
    }
}

```

## Mapper

Allows project to define their own mapping configuration. The project doesn't require AutoMapper to be used but is based on it. To setup get all `Wlvyr.Common.Interface.Configuration.IMapperConfig<TConfigurer>` implementations, from relevant workspace projects, and initialize.

Example usage:

```cs

using AutoMapper;
using Wlvyr.Common.Configuration;
using Wlvyr.Common.Reflection;

namespace MainProject;

public static void Main()
{
    IEnumerable<Assembly> assemblies = AssemblyHelper.GetAssemblies(
                        new HashSet<string>(){ "asssembly.project.name1", "assembly.project.name2" }
                 );

    Func<MapperConfigurationExpression> cfgExpFactory = () => MapperConfigurationExpression();
    Func<MapperConfigurationExpression, IMapper> mapperFactory = (cfg) => new MapperConfiguration(cfg).CreateMapper();   
    Func<IEnumerable<IMapperConfig<MapperConfigurationExpression>>> mapperConfigsFactory = () => assemblies.CreateMapperConfigs<MapperConfigurationExpression>(/*optionAppSettings*/); // from MappingExtensions

    var b = new MapperBootstrap<IMapper, MapperConfigurationExpression>(
        cfgExpFactory, 
        mapperFactory,
        mapperConfigsFactory,
    );

    b.Initialize();

    // b.Mapper; can now be accessed. b.Mapper will throw an error if it has not been initialized.
    // b.Mapper can be registered to a DI container. e.g. diBootstrap.DIContainer.RegisterInstance(b.Mapper);
}
```

Then an implementing IMapperConfig will look like

```cs

// In Some project that doesn't have to be in the same project as MapperBootstrap init nor the main application project.
using SimpleInjector;

using Wlvyr.Common.Interface.Configuration;

namespace SomeProject;

public class SomeMapperConfig : IMapperConfig<MapperConfigurationExpression> {

    public void Configure(MapperConfigurationExpression cfg){
        cfg.CreateMap<Employee, EmployeeDTO>();
    }
}

// if appSettings is not included, in assemblies.CreateMapperConfigs<MapperConfigurationExpression>() this will not be included.
public class SomeMapperConfigWithAppSettingParam : IMapperConfig<MapperConfigurationExpression> {

    public SomeMapperConfigWithAppSettingParam(AppSettings appSettings){
        this.AppSettings = appSettings;

    }
    
    protected AppSettings AppSettings { get; init; } 

    public void Configure(MapperConfigurationExpression cfg){
        cfg.CreateMap<Employee, EmployeeDTO>();
    }
}

```

## License

This project is under MIT License.