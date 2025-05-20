/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;

namespace Wlvyr.Common.Security.Vault.HashiCorp;

/// <summary>
/// Loads configuration data from HashiCorp Vault using the AppRole authentication method.
/// Implements <see cref="ConfigurationProvider"/> to integrate Vault secrets into the ASP.NET Core configuration system.
/// </summary>
public class HashiCorpVaultConfigurationProvider : ConfigurationProvider
{
    private readonly HashiCorpVaultConfigurationOptions _options;
    private IVaultClient _vaultClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashiCorpVaultConfigurationProvider"/> class with the specified Vault configuration options.
    /// </summary>
    /// <param name="options">The Vault configuration options including address, role ID, secret ID, mount point, and secret path.</param>
    /// <exception cref="ArgumentNullException">Thrown if any required option is missing or empty.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public HashiCorpVaultConfigurationProvider(HashiCorpVaultConfigurationOptions options)


#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        this.ValidateOptions(options);
        _options = options;
        this.InitVaultClient(_options);
    }

    /// <summary>
    /// Loads the secret data from Vault and sets it into the configuration provider's data dictionary.
    /// This method blocks on the asynchronous loading internally.
    /// </summary>
    public override void Load()
    {
        // Block on the async call since Load() is synchronous
        var loadTask = LoadSecretsFromVaultAsync();
        loadTask.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Asynchronously loads the secrets from Vault, refreshing the Vault token if needed, then populates the provider's data.
    /// </summary>
    private async Task LoadSecretsFromVaultAsync()
    {
        await EnsureValidTokenAsync();

        var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: _options.Path,
            mountPoint: _options.MountPoint
        );

        if (secret?.Data?.Data is not null)
        {
            Data.Clear();

            foreach (var kvp in secret.Data.Data)
            {
                if (kvp.Value is not null)
                    Data[kvp.Key] = kvp.Value.ToString()!;
            }
        }
        else
        {
            throw new InvalidOperationException("No secret data found at the specified Vault path.");
        }
    }

    /// <summary>
    /// Initializes the Vault client with AppRole authentication using the provided options.
    /// </summary>
    /// <param name="options">Vault configuration options.</param>
    protected void InitVaultClient(HashiCorpVaultConfigurationOptions options)
    {
        var approleAuthMethod = new AppRoleAuthMethodInfo(
            roleId: options.RoleID,
            secretId: options.SecretID
        );

        var vaultClientSettings = new VaultClientSettings(
            options.VaultAddress,
            approleAuthMethod
        );

        this._vaultClient = new VaultClient(vaultClientSettings);
    }

    /// <summary>
    /// Ensures the Vault token is valid and renewable. Attempts to renew the token if it is close to expiration,
    /// or reinitializes the Vault client if renewal fails.
    /// </summary>
    protected async Task EnsureValidTokenAsync()
    {
        var tokenInfo = await _vaultClient.V1.Auth.Token.LookupSelfAsync();

        if (!tokenInfo.Data.Renewable || tokenInfo.Data.TimeToLive < 60)
        {
            try
            {
                await _vaultClient.V1.Auth.Token.RenewSelfAsync();
            }
            catch
            {
                // fallback to full re-login if renewal fails
                this.InitVaultClient(_options);
            }
        }
    }

    /// <summary>
    /// Validates the required Vault configuration options, throwing exceptions if any are missing or invalid.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    /// <exception cref="ArgumentNullException">If any required property is null or empty.</exception>
    protected void ValidateOptions(HashiCorpVaultConfigurationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.VaultAddress))
            throw new ArgumentNullException(nameof(options.VaultAddress));

        if (string.IsNullOrWhiteSpace(options.RoleID))
            throw new ArgumentNullException(nameof(options.RoleID));

        if (string.IsNullOrWhiteSpace(options.SecretID))
            throw new ArgumentNullException(nameof(options.SecretID));

        if (string.IsNullOrWhiteSpace(options.MountPoint))
            throw new ArgumentNullException(nameof(options.MountPoint));

        if (string.IsNullOrWhiteSpace(options.Path))
            throw new ArgumentNullException(nameof(options.Path));
    }
}
