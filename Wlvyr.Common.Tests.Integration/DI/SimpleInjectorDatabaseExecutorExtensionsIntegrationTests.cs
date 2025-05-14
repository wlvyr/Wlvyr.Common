/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Wlvyr.Common.Interface.Configuration;
using System.Data.Common;
using Wlvyr.Common.Data.Configuration;
using Wlvyr.Common.Data;
using SimpleInjector;
using Wlvyr.Common.DI.SimpleInjector;
using Microsoft.Data.SqlClient;
using SimpleInjector.Lifestyles;


namespace Wlvyr.Common.Tests.Integration.DI;

public class SimpleInjectorDatabaseExecutorExtensionsIntegrationTests
{
    private readonly Container _container;

    public SimpleInjectorDatabaseExecutorExtensionsIntegrationTests()
    {
        _container = new Container();
        _container.Options.EnableAutoVerification = false;
        _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
    }

    public interface ISampleRepository
    {
        string GetData();
    }

    public class SampleRepository : ISampleRepository
    {
        private readonly IDatabaseExecutor _executor;

        public SampleRepository(IDatabaseExecutor executor)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public string GetData()
        {
            return _executor is DatabaseExecutor ? "OK" : "FAIL";
        }
    }

    [Fact]
    public void RegisterRepository_TDataRepo_ResolvesSuccessfully()
    {
        // Arrange
        RegisterExecutorDependencies();

        // Act
        _container.RegisterRepository<SampleRepository>();
        using (AsyncScopedLifestyle.BeginScope(_container))
        {
            var repo = _container.GetInstance<SampleRepository>();
            // Assert
            Assert.NotNull(repo);
            Assert.Equal("OK", repo.GetData());
        }
    }

    [Fact]
    public void RegisterRepository_TIRepo_TDataRepo_ResolvesSuccessfully()
    {
        // Arrange
        RegisterExecutorDependencies();

        // Act
        _container.RegisterRepository<ISampleRepository, SampleRepository>();
        using (AsyncScopedLifestyle.BeginScope(_container))
        {
            var repo = _container.GetInstance<ISampleRepository>();
            // Assert
            Assert.NotNull(repo);
            Assert.Equal("OK", repo.GetData());
        }
    }

    [Fact]
    public void RegisterRepository_TDataRepo_ThrowsIfFactoryMissing()
    {
        // Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            _container.RegisterRepository<SampleRepository>();
        });

        Assert.Contains("IDatabaseExecutor", ex.Message);
    }

    [Fact]
    public void RegisterRepository_TIRepo_TDataRepo_ThrowsIfFactoryMissing()
    {
        // Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            _container.RegisterRepository<ISampleRepository, SampleRepository>();
        });

        Assert.Contains("IDatabaseExecutor", ex.Message);
    }

    private void RegisterExecutorDependencies()
    {
        var mockSettings = new Mock<IAppSettings>();
        mockSettings
            .Setup(x => x.GetConnectionString("Default"))
            .Returns("Data Source=(local);Initial Catalog=TestDb;Integrated Security=True");

        var connectionFactory = new Func<string, DbConnection>(_ => new SqlConnection());

        var configProvider = new DatabaseConfigProviderBuilder(mockSettings.Object)
            .SetDefaultConnectionName("Default")
            .SetDefaultConnectionFactory(connectionFactory)
            .SetDefaultExecutorKind(ExecutorKind.Default)
            .Build();

        var executorFactory = new DatabaseExecutorFactory(configProvider);

        _container.RegisterInstance<IDatabaseExecutorFactory>(executorFactory);
        _container.Register<IDatabaseExecutor>(() =>
            executorFactory.Create(typeof(SampleRepository)), Lifestyle.Transient);
    }
}