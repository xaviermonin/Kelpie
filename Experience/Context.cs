using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Context : DbContext
    {
        public Context(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<Context>());
        }

        public DbSet<Wheel> Roues { get; set; }
        public DbSet<Car> Voitures { get; set; }
    }
}
