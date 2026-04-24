using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Data.Configurations.Common;

namespace OrderFlow.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            ConfigureBaseProperties(builder);

            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.Type)
                    .IsRequired();

            builder.Property(o => o.Priority)
                   .IsRequired();

            builder.Property(o => o.ExternalReference)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .IsRequired();

            builder.HasMany(o => o.Events)
                   .WithOne()
                   .HasForeignKey(e => e.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.Events)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Property(o => o.RowVersion)
                    .IsRowVersion();
        }
    }
}