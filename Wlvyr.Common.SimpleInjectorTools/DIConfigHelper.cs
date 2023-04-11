using System.Reflection;

using SimpleInjector;

using Wlvyr.Common.Interface.DI;
using Wlvyr.Common.Interface.Configuration;


namespace Wlvyr.Common.SimpleInjector;


public class DIConfigHelper
{
    public static IEnumerable<IDIConfig<TContainer, TAppSettings>> GetIDIConfigs<TContainer, TAppSettings>(
    Func<TContainer> containerFactory,
    IEnumerable<Assembly> assemblies)
    where TContainer : Container
    where TAppSettings : IAppSettings
    {
        using var tmpDIContainer = containerFactory();
        tmpDIContainer.Register(typeof(IDIConfig<TContainer, TAppSettings>), assemblies);
        IEnumerable<IDIConfig<TContainer, TAppSettings>> diconfigs = tmpDIContainer.GetAllInstances<IDIConfig<TContainer, TAppSettings>>();
        return diconfigs;
    }
}



