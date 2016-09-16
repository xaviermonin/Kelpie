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
            Database.SetInitializer(new CreateDatabaseIfNotExists<Context>());
        }

        public DbSet<Roue> Roues { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
    }
}
