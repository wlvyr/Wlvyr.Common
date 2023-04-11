namespace Wlvyr.Common.Interface.Mappers;

/// <summary>
/// Mapper configuration of the project implementing this interface.
/// </summary>
/// <typeparam name="TConfigurer">The class type that can configure the mapping.</typeparam>
public interface IMapperConfig<TConfigurer>
{
    void Configure(TConfigurer cfg);
}