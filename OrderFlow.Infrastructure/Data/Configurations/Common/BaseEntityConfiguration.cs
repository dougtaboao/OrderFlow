using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Common;

namespace OrderFlow.Infrastructure.Data.Configurations.Common
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);

        protected void ConfigureBaseProperties(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

        }

        //protected void ConfigureGuidId<TAnyEntity>(EntityTypeBuilder<TAnyEntity> builder, string propertyName = "Id")
        //    where TAnyEntity : class
        //{
        //    builder.Property(propertyName)
        //           .ValueGeneratedNever()
        //           .IsRequired();
        //}

        //protected void ConfigureCreatedAt<TAnyEntity>(EntityTypeBuilder<TAnyEntity> builder, string propertyName = "CreatedAt")
        //    where TAnyEntity : class
        //{
        //    builder.Property(propertyName)
        //           .IsRequired();
        //}
    }
}