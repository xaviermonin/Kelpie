using Kelpie.Entity;
using System;
using System.Collections.Generic;

namespace Kelpie.Initialization.Metadata.Models
{
    internal class AttributeType : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AttributeType()
        {
            this.Attributes = new HashSet<Attribute>();
        }

        public string ClrName { get; set; }
        public string SqlServerName { get; set; }
        public int? DefaultLength { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attribute> Attributes { get; set; }
    }
}
