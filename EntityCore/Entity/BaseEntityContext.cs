using EntityCore.DynamicEntity.Event;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EntityCore.Entity
{
    public class BaseEntityContext : DbContext
    {
        public BaseEntityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
    }
}
