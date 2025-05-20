/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Wlvyr.Common.Security.Vault.HashiCorp;

/// <summary>
/// Represents configuration options required to connect to HashiCorp Vault using AppRole authentication.
/// </summary>
public class HashiCorpVaultConfigurationOptions
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Gets or sets the Vault server address (e.g., "https://vault.mycompany.com:8200").
    /// </summary>
    public string VaultAddress { get; set; }


    /// <summary>
    /// Gets or sets the AppRole Role ID used for authentication.
    /// </summary>
    public string RoleID { get; set; }

    /// <summary>
    /// Gets or sets the AppRole Secret ID used for authentication.
    /// </summary>
    public string SecretID { get; set; }

    /// <summary>
    /// Gets or sets the mount point of the KV secrets engine in Vault.
    /// Defaults to "secret".
    /// </summary>
    public string MountPoint { get; set; } = "secret";

    /// <summary>
    /// Gets or sets the path within the KV secrets engine where secrets are stored.
    /// For example, "webapp-template/config".
    /// </summary>
    public string Path { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}

