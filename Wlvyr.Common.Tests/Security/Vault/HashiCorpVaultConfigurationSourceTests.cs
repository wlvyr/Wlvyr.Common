/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Microsoft.Extensions.Configuration;
using Wlvyr.Common.Security.Vault.HashiCorp;
using Xunit;

namespace Wlvyr.Common.Tests.Security.Vault;

public class HashiCorpVaultConfigurationSourceTests
{
    [Fact]
    public void Build_ReturnsHashiCorpVaultConfigurationProvider()
    {
        var options = new HashiCorpVaultConfigurationOptions
        {
            VaultAddress = "http://vault",
            RoleID = "role",
            SecretID = "secret",
            MountPoint = "secret",
            Path = "path"
        };

        var source = new HashiCorpVaultConfigurationSource(options);
        var provider = source.Build(new ConfigurationBuilder());

        Assert.IsType<HashiCorpVaultConfigurationProvider>(provider);
    }
}