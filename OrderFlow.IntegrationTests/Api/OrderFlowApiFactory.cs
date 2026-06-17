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

using Microsoft.AspNetCore.Authentication;

namespace OrderFlow.IntegrationTests.Api
{
    public class OrderFlowApiFactory : WebApplicationFactory<Program>
    {
        private readonly DatabaseFixture _databaseFixture;

        public OrderFlowApiFactory(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<OrderFlowDbContext>));

                if (dbContextDescriptor is not null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<OrderFlowDbContext>(options =>
                {
                    options.UseSqlServer(_databaseFixture.ConnectionString);
                });

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