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
    /// <returns>A list of IMapperConfig implementation.</returns>
    /// <remarks>
    /// <para>Note: Automapper's TConfigurer is MapperConfigurationExpression.</para>
    /// </remarks>
    public static IEnumerable<IMapperConfig<TConfigurer>> CreateMapperConfigs<TConfigurer>(
        this IEnumerable<Assembly> assemblies
    )
    {
        var interfaceType = typeof(IMapperConfig<TConfigurer>);

        return assemblies.SelectMany(a => a.GetTypesSafely())
                         .Where(t => !t.IsAbstract
                                  && !t.IsInterface
                                  && interfaceType.IsAssignableFrom(t))
                         .Select(t => (IMapperConfig<TConfigurer>)Activator.CreateInstance(t)!);

        // SimpleInjector way. keeping for reference.
        // var tempContainer = new SimpleInjector.Container();
        // tempContainer.Collection.Register(typeof(IMapperConfig<MapperConfigurationExpression>), assemblies);
        // return tempContainer.GetAllInstances<IMapperConfig<MapperConfigurationExpression>>();
    }
}