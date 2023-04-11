using System.Reflection;

using Wlvyr.Common.Interface.Mappers;

using AutoMapper;

namespace Wlvyr.Common.AutoMapperTools;

public class MapperConfigHelper
{
    public static IEnumerable<IMapperConfig<MapperConfigurationExpression>> GetMapperConfigs(
    IEnumerable<Assembly> assemblies)
    {
        var tempContainer = new SimpleInjector.Container();
        tempContainer.Collection.Register(typeof(IMapperConfig<MapperConfigurationExpression>), assemblies);
        return tempContainer.GetAllInstances<IMapperConfig<MapperConfigurationExpression>>();
    }

    public static MapperConfigurationExpression CfgExpFactory() => new MapperConfigurationExpression();

    public static IMapper MapperFactory(MapperConfigurationExpression cfg)
    {
        var mapperConfig = new MapperConfiguration(cfg);
        return mapperConfig.CreateMapper();
    }
}

