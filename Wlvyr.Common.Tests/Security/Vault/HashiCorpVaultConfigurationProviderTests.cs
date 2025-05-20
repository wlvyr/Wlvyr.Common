/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.Token.Models;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.KeyValue.V2;
using Wlvyr.Common.Security.Vault.HashiCorp;
using Xunit;

namespace Wlvyr.Common.Tests.Security.Vault;

public class HashiCorpVaultConfigurationProviderTests
{
    [Fact]
    public void Load_ValidData_PopulateDataDictionarySuccess()
    {
        // Arrange
        var options = new HashiCorpVaultConfigurationOptions
        {
            VaultAddress = "http://localhost:8200",
            RoleID = "role-id",
            SecretID = "secret-id",
            MountPoint = "secret",
            Path = "myapp/config"
        };

        var mockVaultClient = new Mock<IVaultClient>();
        var mockSecretsEngine = new Mock<IKeyValueSecretsEngineV2>();

        mockSecretsEngine
            .Setup(m => m.ReadSecretAsync(options.Path, null, options.MountPoint, null))
            .ReturnsAsync(new Secret<SecretData>
            {
                Data = new SecretData
                {
                    Data = new Dictionary<string, object>
                    {
                    { "username", "admin" },
                    { "password", "secret" }
                    }
                }
            });

        mockVaultClient.Setup(m => m.V1.Secrets.KeyValue.V2).Returns(mockSecretsEngine.Object);

        mockVaultClient.Setup(m => m.V1.Auth.Token.LookupSelfAsync())
                       .ReturnsAsync(new Secret<CallingTokenInfo>
                       {
                           Data = new CallingTokenInfo
                           {
                               Renewable = true,
                               TimeToLive = 3600
                           }
                       });

        var provider = new TestableHashiCorpVaultConfigurationProvider(options, mockVaultClient.Object);

        // Act
        provider.Load(); // invokes LoadSecretsFromVaultAsync internally

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        provider.TryGet("username", out string username);
        provider.TryGet("password", out string password);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        // Assert
        Assert.Equal("admin", username);
        Assert.Equal("secret", password);
    }

    [Fact]
    public void Load_ExpiringToken_ReadSecretAsyncInvoked()
    {
        // Arrange
        var options = new HashiCorpVaultConfigurationOptions
        {
            VaultAddress = "http://localhost:8200",
            RoleID = "dummy",
            SecretID = "dummy",
            MountPoint = "secret",
            Path = "somepath"
        };

        var mockVaultClient = new Mock<IVaultClient>();
        var mockTokenAuth = new Mock<ITokenAuthMethod>();
        var mockSecretsEngine = new Mock<IKeyValueSecretsEngineV2>();

        // required to finish the load
        mockSecretsEngine
           .Setup(m => m.ReadSecretAsync(options.Path, null, options.MountPoint, null))
           .ReturnsAsync(new Secret<SecretData>
           {
               Data = new SecretData
               {
                   Data = new Dictionary<string, object>
                   {
                    { "username", "admin" },
                    { "password", "secret" }
                   }
               }
           });

        var tokenInfo = new Secret<CallingTokenInfo>
        {
            Data = new CallingTokenInfo
            {
                Renewable = true,
                TimeToLive = 30  // less than 60, should trigger renewal
            }
        };

        mockVaultClient.Setup(m => m.V1.Secrets.KeyValue.V2).Returns(mockSecretsEngine.Object);
        mockVaultClient.Setup(x => x.V1.Auth.Token).Returns(mockTokenAuth.Object);

        // should not be required.
        // mockVaultClient.Setup(m => m.V1.Auth.Token.LookupSelfAsync()).ReturnsAsync(tokenInfo);

        var authInfo = new VaultSharp.V1.Commons.AuthInfo();

        mockTokenAuth.Setup(x => x.LookupSelfAsync()).ReturnsAsync(tokenInfo);
        mockTokenAuth.Setup(x => x.RenewSelfAsync(null)).ReturnsAsync(authInfo).Verifiable();

        var provider = new TestableHashiCorpVaultConfigurationProvider(options, mockVaultClient.Object);

        // Act
        provider.Load();

        // Assert
        mockTokenAuth.Verify(x => x.RenewSelfAsync(null), Times.Once);
    }

    [Fact]
    public void Load_TokenLifeTimeGood_ReadSecretAsyncNotInvoked()
    {
        // Arrange
        var options = new HashiCorpVaultConfigurationOptions
        {
            VaultAddress = "http://localhost:8200",
            RoleID = "dummy",
            SecretID = "dummy",
            MountPoint = "secret",
            Path = "somepath"
        };

        var mockVaultClient = new Mock<IVaultClient>();
        var mockTokenAuth = new Mock<ITokenAuthMethod>();
        var mockSecretsEngine = new Mock<IKeyValueSecretsEngineV2>();

        // required to finish the load
        mockSecretsEngine
           .Setup(m => m.ReadSecretAsync(options.Path, null, options.MountPoint, null))
           .ReturnsAsync(new Secret<SecretData>
           {
               Data = new SecretData
               {
                   Data = new Dictionary<string, object>
                   {
                    { "username", "admin" },
                    { "password", "secret" }
                   }
               }
           });


        var tokenInfo = new Secret<CallingTokenInfo>
        {
            Data = new CallingTokenInfo
            {
                Renewable = true,
                TimeToLive = 61  // less than 60, should trigger renewal
            }
        };

        mockVaultClient.Setup(m => m.V1.Secrets.KeyValue.V2).Returns(mockSecretsEngine.Object);
        mockVaultClient.Setup(x => x.V1.Auth.Token).Returns(mockTokenAuth.Object);

        var authInfo = new VaultSharp.V1.Commons.AuthInfo();

        mockTokenAuth.Setup(x => x.LookupSelfAsync()).ReturnsAsync(tokenInfo);
        mockTokenAuth.Setup(x => x.RenewSelfAsync(null)).ReturnsAsync(authInfo).Verifiable();

        mockVaultClient.Setup(x => x.V1.Auth.Token).Returns(mockTokenAuth.Object);

        var provider = new TestableHashiCorpVaultConfigurationProvider(options, mockVaultClient.Object);

        // Act
        provider.Load();

        // Assert
        mockTokenAuth.Verify(x => x.RenewSelfAsync(null), Times.Never);
    }

    class TestableHashiCorpVaultConfigurationProvider : HashiCorpVaultConfigurationProvider
    {
        public TestableHashiCorpVaultConfigurationProvider(
            HashiCorpVaultConfigurationOptions options,
            IVaultClient vaultClient)
            : base(options)
        {
            typeof(HashiCorpVaultConfigurationProvider)
            .GetField("_vaultClient", BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(this, vaultClient);
        }
    }
}