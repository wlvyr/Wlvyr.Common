/*---------------------------------------------------------------------------------------------
 *  Copyright (c) wlvyr. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

// Since SimpleInjector is required. this is an integration test.

// using System;
// using Moq;
// using Xunit;
// using SimpleInjector;
// using Wlvyr.Common.Data;
// using Wlvyr.Common.Data.Configuration;

// namespace Wlvyr.Common.Tests.Integration.DI;

// public class SimpleInjectorDatabaseExecutorExtensionsTests
// {
//     public class TestRepo
//     {
//         public IDatabaseExecutor Executor { get; }
//         public TestRepo(IDatabaseExecutor executor) => Executor = executor;
//     }

//     public interface ITestRepo { }

//     public class TestRepoWithInterface : ITestRepo
//     {
//         public IDatabaseExecutor Executor { get; }
//         public TestRepoWithInterface(IDatabaseExecutor executor) => Executor = executor;
//     }

//     [Fact]
//     public void RegisterRepository_ShouldThrow_WhenFactoryNotRegistered()
//     {
//         // Arrange
//         var container = new Container();

//         // Act & Assert
//         var ex = Assert.Throws<InvalidOperationException>(() => container.RegisterRepository<TestRepo>());

//         Assert.Contains("IDatabaseExecutorFactory", ex.Message);
//     }

//     [Fact]
//     public void RegisterRepository_ShouldResolveConcrete_WithExecutor()
//     {
//         // Arrange
//         var container = new Container();

//         var expectedExecutor = new Mock<IDatabaseExecutor>().Object;
//         var mockFactory = new Mock<IDatabaseExecutorFactory>();
//         mockFactory
//             .Setup(f => f.Create(typeof(TestRepo)))
//             .Returns(expectedExecutor);

//         container.RegisterInstance(mockFactory.Object);

//         // Act
//         container.RegisterRepository<TestRepo>();
//         container.Verify();

//         var instance = container.GetInstance<TestRepo>();

//         // Assert
//         Assert.NotNull(instance);
//         Assert.Equal(expectedExecutor, instance.Executor);
//     }

//     [Fact]
//     public void RegisterRepository_WithInterface_ShouldResolveBoth_WithExecutor()
//     {
//         // Arrange
//         var container = new Container();

//         var expectedExecutor = new Mock<IDatabaseExecutor>().Object;
//         var mockFactory = new Mock<IDatabaseExecutorFactory>();
//         mockFactory
//             .Setup(f => f.Create(typeof(TestRepoWithInterface)))
//             .Returns(expectedExecutor);

//         container.RegisterInstance(mockFactory.Object);

//         // Act
//         container.RegisterRepository<ITestRepo, TestRepoWithInterface>();
//         container.Verify();

//         var iface = container.GetInstance<ITestRepo>();
//         var concrete = container.GetInstance<TestRepoWithInterface>();

//         // Assert
//         Assert.NotNull(iface);
//         Assert.NotNull(concrete);
//         Assert.Same(iface, concrete);
//         Assert.Equal(expectedExecutor, concrete.Executor);
//     }
// }