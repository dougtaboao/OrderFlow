using System;
using System.Collections.Generic;
using System.Text;

namespace OrderFlow.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected BaseEntity()
        {
                
        }

        protected BaseEntity(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                throw new ArgumentException("Id Inválido");
            }

            Id = id;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
