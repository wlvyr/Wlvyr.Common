# Wlvyr.Common

Wlvyr.Common is a library created to aid in common project setup.

## Projects

- Wlvyr.Common
  - A foundational library providing utilities and helpers to streamline common project setup tasks, such as dependency injection configuration and shared services.

- Wlvyr.Common.Interface
  - A lightweight library containing only the interfaces used by Wlvyr.Common. This allows projects that depend solely on interface contracts to avoid referencing the full Wlvyr.Common library.

- Wlvyr.Common.Data
  - is a reusable library that provides an ADO.NET-agnostic abstraction for executing database operations. It encapsulates common patterns for interacting with relational databases while remaining decoupled from specific providers (e.g., SQL Server, PostgreSQL, MySQL).

- Wlvyr.Common.DI.SimpleInjector
  - A helper library that simplifies common setup patterns and configurations for the SimpleInjector dependency injection container.

## Project Notes

- For now, direct project referencing should be preferable. Only when two or more projects are affected (needs to be updated but shouldn't) because another project is being updated should probably be the time to only reference via nuget.

- Reference structure
  - Wlvyr.Common.Interface is referenced by Wlvyr.Common and Wlvyr.Common.Data
  - Wlvyr.Common.Data is referenced by Wlvyr.Common.DI.SimpleInjector

## License

This project is under MIT License. Please see [LICENSE](https://github.com/wlvyr/Wlvyr.Common/blob/master/LICENSE) for details.
