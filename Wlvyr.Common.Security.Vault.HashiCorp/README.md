# Wlvyr.Common.Security.Vault.HashiCorp

The Wlvyr.Common.Security.Vault.HashiCorp namespace provides integration between .NET's configuration system and HashiCorp Vault, enabling secrets and configuration data to be sourced securely from Vault using the standard IConfigurationBuilder pipeline.

## Usage

```cs
using Wlvyr.Common.Security.Vault.HashiCorp;

builder.Configuration.AddHashiCorpVault(options =>
{
    // Configuration values sourced from environment variables
    options.VaultAddress = builder.Configuration["VAULT_API_ADDR"]!;
    options.RoleID = builder.Configuration["VAULT_ROLE_ID"]!;
    options.SecretID = builder.Configuration["VAULT_SECRET_ID"]!;
    options.Path = builder.Configuration["VAULT_PATH"]!;
});
```

Upon building the configuration, the `HashiCorpVaultConfigurationProvider` will load secrets from HashiCorp Vault and inject them into the application's configuration system. These secrets become accessible via `builder.Configuration` (and later `app.Configuration`), just like any other configuration source.
