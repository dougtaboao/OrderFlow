using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Data.Configurations.Common;

namespace OrderFlow.Infrastructure.Data.Configurations
{
    public class OrderEventConfiguration : BaseEntityConfiguration<OrderEvent>
    {
        public override void Configure(EntityTypeBuilder<OrderEvent> builder)
        {
            builder.ToTable("OrderEvents");

            builder.HasKey(e => e.Id);

            ConfigureBaseProperties(builder);

            // ConfigureGuidId(builder); Centralizado no metodo acima -ConfigureBaseProperties(builder);
            // ConfigureCreatedAt(builder); Centralizado no metodo acima -ConfigureBaseProperties(builder);

            builder.Property(e => e.OrderId)
                   .IsRequired();

            builder.Property(e => e.Type)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasMaxLength(500)
                   .IsRequired();
        }
    }
}