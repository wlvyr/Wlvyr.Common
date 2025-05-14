/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Runtime.InteropServices.Marshalling;
using SimpleInjector;
using Wlvyr.Common.Data;
using Wlvyr.Common.Data.Configuration;

namespace Wlvyr.Common.DI.SimpleInjector;

public static class SimpleInjectorDatabaseExecutorExtensions
{
    //// usage 
    /// container.RegisterRepository<AccountRepository>();
    /// container.RegisterRepository<EventRepository>();


    /// <summary>
    /// Registers a repository class that depends on <see cref="IDatabaseExecutor"/> constructed via <see cref="IDatabaseExecutorFactory"/>.
    /// 
    /// This method only registers the concrete repository type.
    /// 
    /// ⚠️ Note: If you also register an interface (e.g., IAccountRepository), make sure both resolve through the same logic.
    /// Otherwise, SimpleInjector will treat them as separate registrations with potentially different constructor paths.
    /// </summary> 
    public static void RegisterRepository<TDataRepo>(this Container container)
        where TDataRepo : class
    {
        var registration = container.GetRegistration(typeof(IDatabaseExecutorFactory), throwOnFailure: false);
        if (registration is null)
        {
            throw new InvalidOperationException(
                $"{nameof(IDatabaseExecutorFactory)} must be registered before calling RegisterDatabaseExecutor.");
        }

        container.Register<TDataRepo>(() =>
        {
            var factory = container.GetInstance<IDatabaseExecutorFactory>();
            var executor = factory.Create(typeof(TDataRepo));
            return (TDataRepo)Activator.CreateInstance(typeof(TDataRepo), executor)!;
        }, Lifestyle.Scoped);
    }
    
    /// <summary>
    /// Registers a repository interface and its implementation using a database executor from <see cref="IDatabaseExecutorFactory"/>.
    /// 
    /// This ensures both the interface and concrete type resolve through the same instance logic.
    /// 
    /// ✅ Recommended when you want to inject the interface (e.g., IAccountRepository) elsewhere.
    /// 
    /// ⚠️ Avoid using Register<IRepo, Repo>() separately from Register<Repo>(...) as it can lead to inconsistent behavior.
    /// </summary>
    public static void RegisterRepository<TIRepo, TDataRepo>(this Container container)
        where TIRepo : class
        where TDataRepo : class, TIRepo

    {
        var registration = container.GetRegistration(typeof(IDatabaseExecutorFactory), throwOnFailure: false);
        if (registration is null)
        {
            throw new InvalidOperationException(
                $"{nameof(IDatabaseExecutorFactory)} must be registered before calling RegisterDatabaseExecutor.");
        }

        container.Register<TIRepo>(() =>
        {
            var factory = container.GetInstance<IDatabaseExecutorFactory>();
            var executor = factory.Create(typeof(TDataRepo));
            return (TDataRepo)Activator.CreateInstance(typeof(TDataRepo), executor)!;
        }, Lifestyle.Scoped);

        container.Register<TDataRepo>(() => (TDataRepo)container.GetInstance<TIRepo>(), Lifestyle.Scoped);
    }
}