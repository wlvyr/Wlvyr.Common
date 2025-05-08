/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/


namespace Wlvyr.Common.Interface.Configuration;

/// <summary>
/// Mapper configuration of the project implementing this interface.
/// </summary>
/// <typeparam name="TConfigurer">The class type that can configure the mapping.</typeparam>
public interface IMapperConfig<TConfigurer>
{
    void Configure(TConfigurer cfg);
}