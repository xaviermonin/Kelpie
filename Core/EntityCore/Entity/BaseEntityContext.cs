using System.Data.Common;
using System.Data.Entity;

namespace EntityCore.Entity
{
    public class BaseEntityContext : DbContext
    {
        public BaseEntityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public BaseEntityContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            
        }
    }
}
