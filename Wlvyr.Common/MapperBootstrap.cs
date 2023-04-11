using Wlvyr.Common.Interface.Mappers;

namespace Wlvyr.Common;

/// <summary>
/// The mapper bootstrap helps in initializing your auto mapping. 
/// </summary>
/// <typeparam name="TMapper">The mapper type to use (e.g. automapper instance)</typeparam>
/// <typeparam name="TConfigurer">The mapper config builder type to use. Would most likely be the config builder of TMapper.</typeparam>
public class MapperBootstrap<TMapper, TConfigurer> : IBootstrap
{

    public MapperBootstrap(
        Func<TConfigurer> cfgExpFactory,
        Func<TConfigurer, TMapper> mapperFactory,
        Func<IEnumerable<IMapperConfig<TConfigurer>>> mapperConfigsFactory)
    {
        this.CfgExpFactory = cfgExpFactory;
        MapperFactory = mapperFactory;
        this.MapperConfigsFactory = mapperConfigsFactory;
    }

    protected TMapper? _mapper;
    /// <summary>
    /// The mapper object containing the mapping configurations and allows its use to automap from one type to another.
    /// </summary>
    /// <returns></returns>
    public TMapper Mapper { get => _mapper ?? throw new ArgumentNullException($"{nameof(Mapper)} is null. Have you tried Initializing the {nameof(MapperBootstrap<TMapper, TConfigurer>)}?."); }
    
    protected Func<TConfigurer> CfgExpFactory { get; }
    protected Func<TConfigurer, TMapper> MapperFactory { get; }
    protected Func<IEnumerable<IMapperConfig<TConfigurer>>> MapperConfigsFactory { get; }

    public void Initialize()
    {
        var autoMapperConfigs = this.MapperConfigsFactory();

        if (!autoMapperConfigs.Any())
        {
            return;
        }

        var cfg = this.CfgExpFactory();
        foreach (var config in autoMapperConfigs)
        {
            config.Configure(cfg);
        }

        this._mapper = this.MapperFactory(cfg);
    }
}