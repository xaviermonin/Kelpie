using Kelpie.Entity;

namespace Kelpie.Initialization.Metadata.Models
{
    internal class Proxy : BaseEntity
    {
        public Entity Entity { get; set; }

        public string FullyQualifiedTypeName { get; set; }

        public bool Managed { get; set; }
    }
}
