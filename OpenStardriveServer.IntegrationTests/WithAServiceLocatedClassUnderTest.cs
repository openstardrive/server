using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenStardriveServer.Domain.Database;
using OpenStardriveServer.Domain.Systems;

namespace OpenStardriveServer.IntegrationTests
{
    public class WithAServiceLocatedClassUnderTest<T> where T : class
    {
        private readonly ServiceCollection serviceCollection;
        private ServiceProvider serviceProvider;

        protected T ClassUnderTest { get; private set; }

        protected WithAServiceLocatedClassUnderTest()
        {
            serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection);
            serviceCollection.AddTransient<T, T>();
        }

        [SetUp]
        public void BaseSetUp()
        {
            serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<ISqliteDatabaseInitializer>().Initialize();
            serviceProvider.GetRequiredService<IRegisterSystemsCommand>().Register();
            ClassUnderTest = serviceProvider.GetService<T>();
        }

        [TearDown]
        public void BaseTearDown()
        {
            
        }

        private static void RegisterServices(ServiceCollection services)
        {
            services.AddLogging();
            DependencyInjectionConfig.ConfigureServices(services);
            services.AddSingleton(x => new SqliteDatabase
            {
                ConnectionString = $"Data Source=integration-tests-{Guid.NewGuid()}.sqlite"
            });
        }
    }
}