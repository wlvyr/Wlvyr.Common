using Wlvyr.Common.Interface.Configuration;

namespace Wlvyr.Common.Interface.DI;

/// <summary>
/// DI Configuration of the project implementing this interface.
/// </summary>
/// <typeparam name="TContainer">The DI container.</typeparam>
/// <typeparam name="TSettings">App settings required to be used alongside the DI container.</typeparam>
public interface IDIConfig<TContainer, TSettings>
where TSettings : IAppSettings
{
    void Configure(TContainer container, TSettings configuration);
}