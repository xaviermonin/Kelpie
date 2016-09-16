using EntityCore.Entity;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class Interface : BaseEntity
    {
        public Entity Entity { get; set; }

        public string FullyQualifiedTypeName { get; set; }

        public bool Managed { get; set; }
    }
}
