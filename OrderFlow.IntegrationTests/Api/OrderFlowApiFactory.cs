using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Security;
using OrderFlow.Infrastructure.Data;
using OrderFlow.IntegrationTests.Fakes;
using OrderFlow.IntegrationTests.Fixtures;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

using Microsoft.AspNetCore.Authentication;

namespace OrderFlow.IntegrationTests.Api
{
    public class OrderFlowApiFactory : WebApplicationFactory<Program>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public OrderFlowApiFactory(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Redis:ConnectionString"] = _integrationTestFixture.RedisConnectionString,
                    ["Redis:OrderCacheExpirationMinutes"] = "5"
                });
            });

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrderFlowDbContext>));

                if (dbContextDescriptor is not null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<OrderFlowDbContext>(options =>
                {
                    options.UseSqlServer(_integrationTestFixture.SqlServerConnectionString);
                });

                services.RemoveAll<IConnectionMultiplexer>();

                services.AddSingleton<IConnectionMultiplexer>(_ =>
                    ConnectionMultiplexer.Connect(_integrationTestFixture.RedisConnectionString));

                services.RemoveAll<ICurrentUser>();

                services.AddScoped<ICurrentUser, FakeCurrentUser>();

                services
                    .AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                        "Test",
                        options => { });

                
            });
        }
    }
}