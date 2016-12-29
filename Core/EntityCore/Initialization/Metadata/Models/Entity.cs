using EntityCore.Entity;
using System.Collections.Generic;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class Entity : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Entity()
        {
            this.Attributes = new HashSet<Attribute>();
            this.Proxies = new HashSet<Proxy>();
            this.OneToManyRelationships = new HashSet<Relationship>();
            this.ManyToOneRelationships = new HashSet<Relationship>();
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool Managed
        {
            get;
            set;
        }

        public bool Metadata
        {
            get;
            set;
        }

        public bool Association
        {
            get;
            set;
        }

        public virtual ICollection<Attribute> Attributes { get; set; }

        public virtual ICollection<Relationship> OneToManyRelationships { get; set; }
        public virtual ICollection<Relationship> ManyToOneRelationships { get; set; }

        public virtual ICollection<Proxy> Proxies { get; set; }
        public virtual ICollection<Listener> Listeners { get; set; }
    }
}
