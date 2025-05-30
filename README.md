# Wlvyr.Common

**Wlvyr.Common** is a foundational library designed to aid in common .NET project setup.

## Projects

- **Wlvyr.Common**
  - A foundational library providing utilities and helpers to streamline common project setup tasks, such as dependency injection configuration and shared services.
  - 📦 [Install via NuGet](https://www.nuget.org/packages/Wlvyr.Common/)  
  - 📘 [README](Wlvyr.Common/README.md)

- **Wlvyr.Common.Interface**
  - A lightweight library containing only the interfaces used by Wlvyr.Common. This allows projects that depend solely on interface contracts to avoid referencing the full Wlvyr.Common library.
  - 📦 [Install via NuGet](https://www.nuget.org/packages/Wlvyr.Common.Interface/)
  - 📘 [README](Wlvyr.Common.Interface/README.md)

- **Wlvyr.Common.Data**
  - is a reusable library that provides an ADO.NET-agnostic abstraction for executing database operations. It encapsulates common patterns for interacting with relational databases while remaining decoupled from specific providers (e.g., SQL Server, PostgreSQL, MySQL).
  - 📦 [Install via NuGet](https://www.nuget.org/packages/Wlvyr.Common.Data/)  
  - 📘 [README](Wlvyr.Common.Data/README.md)

- **Wlvyr.Common.DI.SimpleInjector**
  - A helper library that simplifies common setup patterns and configurations for the [SimpleInjector](https://simpleinjector.org/) dependency injection container.
  - 📦 [Install via NuGet](https://www.nuget.org/packages/Wlvyr.Common.DI.SimpleInjector/)  
  - 📘 [README](Wlvyr.Common.DI.SimpleInjector/README.md)

- **Wlvyr.Common.Security.Vault.HashiCorp**
  - Provides configuration extensions for integrating [HashiCorp Vault](https://developer.hashicorp.com/vault) as a secure configuration source.
  - 📦 [Install via NuGet](https://www.nuget.org/packages/Wlvyr.Common.Security.Vault.HashiCorp/)  
  - 📘 [README](Wlvyr.Common.Security.Vault.HashiCorp/README.md)

## Project Notes
  
- **Dependency Reference:**
  - `Wlvyr.Common.Interface` → used by `Wlvyr.Common` and `Wlvyr.Common.Data`
  - `Wlvyr.Common.Data` → used by `Wlvyr.Common.DI.SimpleInjector`

## Maintenance status

This project is maintained on a best-effort basis.<br>
Updates may be infrequent due to limited available time.<br>
Issues and feature requests are welcome, but responses and releases may take time.<br>
Pull requests are not currently accepted.

## License

This project is under the MIT License.
See [LICENSE](./LICENSE) for details.
