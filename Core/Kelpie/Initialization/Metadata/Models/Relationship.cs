using Kelpie.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kelpie.Initialization.Metadata.Models
{
    internal class Relationship : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Relationship()
        {
            
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public int CascadeDelete { get; set; }
        public bool Managed { get; set; }
        public bool Metadata { get; set; }

        public string OneNavigationName { get; set; }
        public string ManyNavigationName { get; set; }

        [InverseProperty("OneToManyRelationships")]
        public virtual Entity One { get; set; }
        [InverseProperty("ManyToOneRelationships")]
        public virtual Entity Many { get; set; }
    }
}
