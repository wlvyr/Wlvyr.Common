/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Configuration;

namespace Wlvyr.Common.Security.Vault.HashiCorp;

/// <summary>
/// Provides extension methods for integrating HashiCorp Vault into the application's configuration system.
/// </summary>
public static class HashiCorpVaultConfigurationExtensions
{
    /// <summary>
    /// Adds Vault as a configuration source to the <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="builder">The configuration builder to add Vault configuration to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="HashiCorpVaultConfigurationOptions"/>.</param>
    /// <returns>The updated <see cref="IConfigurationBuilder"/>.</returns>
    public static IConfigurationBuilder AddHashiCorpVault(this IConfigurationBuilder builder, Action<HashiCorpVaultConfigurationOptions> configureOptions)
    {
        var options = new HashiCorpVaultConfigurationOptions();
        configureOptions(options);
        return builder.Add(new HashiCorpVaultConfigurationSource(options));
    }
}
