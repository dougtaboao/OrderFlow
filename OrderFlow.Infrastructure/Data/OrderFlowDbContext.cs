using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Interfaces;
using OrderFlow.Domain.ReadModels;

namespace OrderFlow.Infrastructure.Data
{
    public class OrderFlowDbContext : DbContext, IUnitOfWork
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<OrderAuditReadModel> OrderAuditReadModels => Set<OrderAuditReadModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderFlowDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}