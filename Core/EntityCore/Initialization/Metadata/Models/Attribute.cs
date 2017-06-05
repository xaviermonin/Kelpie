using EntityCore.Entity;
using System;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class Attribute : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Attribute()
        {
            this.IsNullable = false;
        }

        public string Name { get; set; }

        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int? Length { get; set; }

        public bool Managed { get; set; }
        public bool Metadata { get; set; }

        public virtual AttributeType Type { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
