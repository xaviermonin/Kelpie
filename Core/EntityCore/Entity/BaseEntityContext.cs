using System.Data.Entity;

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
