using EntityCore.Entity;
using EntityCore.Initialization.Metadata;
using System.Data.Entity;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore
{
    internal partial class MetadataContext : BaseEntityContext
    {
        public MetadataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Database.SetInitializer(new MetadataInitializer());
        }

        public virtual DbSet<Models.Entity> Entities { get; set; }
        public virtual DbSet<Models.Attribute> Attributes { get; set; }
        public virtual DbSet<Models.AttributeType> AttributeTypes { get; set; }
        public virtual DbSet<Models.Interface> Interfaces { get; set; }
    }
}
