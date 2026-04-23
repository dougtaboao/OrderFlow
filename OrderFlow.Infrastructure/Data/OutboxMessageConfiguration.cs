using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Data.Configurations.Common;

namespace OrderFlow.Infrastructure.Data.Configurations
{
    public class OutboxMessageConfiguration : BaseEntityConfiguration<OutboxMessage>
    {
        public override void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            ConfigureBaseProperties(builder);

            builder.Property(x => x.Type)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Payload)
                   .IsRequired();

            builder.Property(x => x.CorrelationId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.ProcessedAt)
                   .IsRequired(false);

            builder.Property(x => x.Error)
                   .HasMaxLength(1000)
                   .IsRequired(false);
        }
    }
}