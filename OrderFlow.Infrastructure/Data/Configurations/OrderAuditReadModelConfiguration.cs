using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.ReadModels;

namespace OrderFlow.Infrastructure.Data.Configurations
{
    public class OrderAuditReadModelConfiguration : IEntityTypeConfiguration<OrderAuditReadModel>
    {
        public void Configure(EntityTypeBuilder<OrderAuditReadModel> builder)
        {
            builder.ToTable("OrderAuditReadModels");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.EventType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.OccurredAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.CorrelationId);
            builder.HasIndex(x => x.EventType);
        }
    }
}