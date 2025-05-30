# Wlvyr.Common.Interface

Wlvyr.Common.Interface is a library that contains only interfaces used in Wlvyr.Common, aiming to decouple Wlvyr.Common from projects that only require the interfaces.

## Dependency Injection

To help facilitate in loading dependency injection (DI) service across different projects, `Wlvyr.Common.Interface.Configuration.IDIConfig<TContainer, TSettings>` provides an interface in configuring your DI in all required projects. Then, binding and initializing the DI configurations from `Wlvyr.Common.Configuration.DIBootstrap<TContainer, TAppSettings>`.

In a project that may not require Wlvyr.Common but only its interface, `Wlvyr.Common.Interface`, an implementation of `IDIConfig` will look like this:

```cs

using SimpleInjector;

using Wlvyr.Common.Interface.Configuration;

namespace SomeProject;

public class SomeDIConfig : IDIConfig<Container, SomeAppSetting> {

    public void Configure(Container container, SomeAppSetting configuration){
        container.Register<ILogger, FileLogger>();
    }
}

```

Please see [Wlvyr.Common](../Wlvyr.Common/README.md#dependency-injection) to see how `IDIConfig` is used.

## Mapper

Allow projects to define their own mapping configuration. The project doesn't require AutoMapper to be used, but is based on it. To setup, get all `Wlvyr.Common.Interface.Configuration.IMapperConfig<TConfigurer>` implementations, from relevant workspace projects, and then initialize.

In a project that may not require Wlvyr.Common but only its interface, `Wlvyr.Common.Interface`, an implementation of `IMapperConfig` will look like this:

```cs

using SimpleInjector;

using Wlvyr.Common.Interface.Configuration;

namespace SomeProject;

public class SomeMapperConfig : IMapperConfig<MapperConfigurationExpression> {

    public void Configure(MapperConfigurationExpression cfg){
        cfg.CreateMap<Employee, EmployeeDTO>();
    }
}

// If appSettings is not included in assemblies.CreateMapperConfigs<MapperConfigurationExpression>(), this will not be included.
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

Please see [Wlvyr.Common](../Wlvyr.Common/README.md#mapper) to see how `IMapperConfig` is used.
