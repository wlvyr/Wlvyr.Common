/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Reflection;
using Wlvyr.Common.Interface.Configuration;
using Wlvyr.Common.Reflection;

namespace Wlvyr.Common.Configuration;


public static class MappingExtensions
{
    /// <summary>
    /// Creates and returns all <see cref="IMapperConfig<TConfigurer>"/> found in the provided assemblies.
    /// </summary>
    /// <typeparam name="TConfigurer"></typeparam>
    /// <param name="assemblies">Project assemblies containing IMapperConfig</param>
    /// <param name="appSettings"></param>
    /// <returns>A list of IMapperConfig implementation.</returns>
    /// <remarks>
    /// <para>Note: Automapper's TConfigurer is MapperConfigurationExpression.</para>
    /// 
    /// <para>IMapperConfig<TConfigurer> can have a constructor with one parameter, of type IAppSettings.</para>
    /// 
    /// <para>When appSettings is null, IMapperConfig<TConfigurer> that require it will not be included.</para>
    /// </remarks>
    public static IEnumerable<IMapperConfig<TConfigurer>> CreateMapperConfigs<TConfigurer>(
        this IEnumerable<Assembly> assemblies,
        IAppSettings appSettings = default!
    )
    {
        var interfaceType = typeof(IMapperConfig<TConfigurer>);

        return assemblies.SelectMany(a => a.GetTypesSafely())
                         .Where(t => IsValidMapperType<TConfigurer>(t, interfaceType))
                         .Where(t => CanBeInstantiatedWith(t, appSettings))
                         .Select(t => InstantiateMapper<TConfigurer>(t, appSettings));

        // SimpleInjector way. keeping for reference.
        // var tempContainer = new SimpleInjector.Container();
        // tempContainer.RegisterInstance(appSettings);
        // tempContainer.Collection.Register(typeof(IMapperConfig<MapperConfigurationExpression>), assemblies);
        // return tempContainer.GetAllInstances<IMapperConfig<MapperConfigurationExpression>>();
    }

    private static bool IsValidMapperType<TConfigurer>(Type type, Type interfaceType)
    {
        return !type.IsAbstract &&
               !type.IsInterface &&
               interfaceType.IsAssignableFrom(type);
    }

    private static bool CanBeInstantiatedWith<TAppSettings>(Type type, TAppSettings appSettings)
    where TAppSettings : IAppSettings
    {
        if (appSettings == null)
        {
            // Only allow types with parameterless constructor
            return type.GetConstructors().Any(c => c.GetParameters().Length == 0);
        }

        // Allow either parameterless or compatible constructor
        return type.GetConstructors().Any(c =>
        {
            var parameters = c.GetParameters();
            return parameters.Length == 0 ||
                   (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(appSettings.GetType()));
        });
    }


    private static IMapperConfig<TConfigurer> InstantiateMapper<TConfigurer>(
        Type type,
        IAppSettings appSettings
    )
    {
        if (appSettings != null)
        {
            var ctor = type.GetConstructors()
                           .FirstOrDefault(c =>
                           {
                               var parameters = c.GetParameters();
                               return parameters.Length == 1 &&
                                      parameters[0].ParameterType.IsAssignableFrom(appSettings.GetType());
                           });

            if (ctor != null)
            {
                return (IMapperConfig<TConfigurer>)ctor.Invoke(new object[] { appSettings })!;
            }
        }

        // Fallback to parameterless constructor
        return (IMapperConfig<TConfigurer>)Activator.CreateInstance(type)!;
    }
}