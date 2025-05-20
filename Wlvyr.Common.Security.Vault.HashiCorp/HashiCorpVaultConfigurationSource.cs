/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Configuration;

namespace Wlvyr.Common.Security.Vault.HashiCorp;

/// <summary>
/// Represents a configuration source that loads configuration data from HashiCorp Vault.
/// </summary>
public class HashiCorpVaultConfigurationSource : IConfigurationSource
{
    private readonly HashiCorpVaultConfigurationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashiCorpVaultConfigurationSource"/> class with the specified Vault configuration options.
    /// </summary>
    /// <param name="options">The options required to connect and authenticate with Vault.</param>
    public HashiCorpVaultConfigurationSource(HashiCorpVaultConfigurationOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Builds the <see cref="IConfigurationProvider"/> responsible for fetching configuration from Vault.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <returns>An instance of <see cref="HashiCorpVaultConfigurationProvider"/>.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new HashiCorpVaultConfigurationProvider(_options);
    }
}
