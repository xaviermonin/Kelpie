using EntityCore.Entity;
using EntityCore.Initialization.Metadata;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore
{
    internal partial class MetadataContext : BaseEntityContext
    {
        public MetadataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            SetInitializer();
        }

        public MetadataContext(DbConnection existingConnection, bool contextOwnsConnection)
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
        public virtual DbSet<Models.Proxy> Interfaces { get; set; }
    }
}
