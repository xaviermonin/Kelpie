using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EntityCore.Entity
{
    public class BaseEntityContext : DbContext
    {
        public BaseEntityContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {

        }

        public BaseEntityContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            
        }
    }
}
