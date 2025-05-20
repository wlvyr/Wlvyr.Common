/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Linq;
using Microsoft.Extensions.Configuration;
using Wlvyr.Common.Security.Vault.HashiCorp;
using Xunit;

namespace Wlvyr.Common.Tests.Security.Vault;

public class HashiCorpVaultConfigurationExtensionsTests
{
    [Fact]
    public void AddHCPVault_Should_Register_HashiCorpVaultConfigurationSource()
    {
        // Arrange
        var builder = new ConfigurationBuilder();

        // Act
        builder.AddHashiCorpVault(options =>
        {
            options.VaultAddress = "http://vault";
            options.RoleID = "role";
            options.SecretID = "secret";
            options.Path = "path";
        });

        // Assert
        var source = builder.Sources.OfType<HashiCorpVaultConfigurationSource>().FirstOrDefault();
        Assert.NotNull(source);
    }
}