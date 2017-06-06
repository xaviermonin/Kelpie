using Kelpie.Entity;
using Kelpie.Initialization.Metadata;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Models = Kelpie.Initialization.Metadata.Models;

namespace Kelpie.Initialization.Metadata
{
    internal partial class MetadataInitializationContext : DbContext
    {
        public MetadataInitializationContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            SetInitializer();
        }

        public MetadataInitializationContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            SetInitializer();
        }

        private void SetInitializer()
        {
            Database.SetInitializer(new MetadataInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public virtual DbSet<Models.Entity> Entities { get; set; }
        public virtual DbSet<Models.Attribute> Attributes { get; set; }
        public virtual DbSet<Models.AttributeType> AttributeTypes { get; set; }
        public virtual DbSet<Models.Proxy> Proxies { get; set; }
        public virtual DbSet<Models.Listener> Listeners { get; set; }
    }
}
