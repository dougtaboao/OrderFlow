using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderFlow.Infrastructure.Data.Configurations.Common
{
    public abstract class BaseConfigutationTreino<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class 
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    } 
}
