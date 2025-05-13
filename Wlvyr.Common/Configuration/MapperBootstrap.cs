/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Configuration;

/// <summary>
/// The mapper bootstrap helps in initializing your object mapping. 
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
        this.CfgExpFactory = cfgExpFactory ?? throw new ArgumentNullException(nameof(cfgExpFactory));
        this.MapperFactory = mapperFactory ?? throw new ArgumentNullException(nameof(mapperFactory));
        this.MapperConfigsFactory = mapperConfigsFactory ?? throw new ArgumentNullException(nameof(mapperConfigsFactory));
    }

    protected TMapper? _mapper;

    /// <summary>
    /// The mapper object containing the mapping configurations and allows its use to automap from one type to another.
    /// </summary>
    /// <returns></returns>
    public TMapper Mapper
    {
        get => _mapper ??
                                        throw new ArgumentNullException($"{nameof(Mapper)} is null. Have you tried invoking, Initialize, of {nameof(MapperBootstrap<TMapper, TConfigurer>)}?.");
    }

    protected Func<TConfigurer> CfgExpFactory { get; }
    protected Func<TConfigurer, TMapper> MapperFactory { get; }
    protected Func<IEnumerable<IMapperConfig<TConfigurer>>> MapperConfigsFactory { get; }

    public void Initialize()
    {
        var autoMapperConfigs = this.MapperConfigsFactory() ?? Enumerable.Empty<IMapperConfig<TConfigurer>>();
        var cfg = this.CfgExpFactory();

        foreach (var config in autoMapperConfigs)
        {
            config.Configure(cfg);
        }

        this._mapper = this.MapperFactory(cfg);
    }
}