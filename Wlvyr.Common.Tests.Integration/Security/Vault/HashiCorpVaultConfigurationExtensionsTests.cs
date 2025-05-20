
/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Microsoft.Extensions.Configuration;
using Wlvyr.Common.Security.Vault.HashiCorp;
using Xunit;

namespace Wlvyr.Common.Tests.Integration.Security.Vault;

public class HashiCorpVaultConfigurationExtensionsTests
{
    // Integration test
    [Fact]
    public void AddHCPVault_ValidWorkingConfiguration_VaultDataAccessible()
    {
        var builder = new ConfigurationBuilder();

        builder.AddHashiCorpVault(options =>
        {
            options.VaultAddress = "http://vault-container:8200";
            options.RoleID = "dd3ce8c3-71b7-9d6e-f3ea-7c3ffc0d72e1";
            options.SecretID = "cf74c704-dad5-5ee4-30a5-52b9ec7c5821";
            options.Path = "webapp-template/config";
        });

        // If this fails. the possible causes are
        // - vault is not running -- ensure vault-container is running
        // - invalid url -- ensure this project is in the network of the vault-container
        // - invalid path, roleId, secretId -- the vault might have reset.
        var config = builder.Build();

        Assert.NotNull(config);
        // You can also inspect internal sources if needed
        Assert.Contains(builder.Sources, s => s is HashiCorpVaultConfigurationSource);

        Assert.NotNull(config["Connectionstrings:PlatformConnectionUser"]);
    }
}