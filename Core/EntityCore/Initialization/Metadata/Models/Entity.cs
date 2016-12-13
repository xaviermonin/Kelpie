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

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private bool _managed;
        public bool Managed
        {
            get { return _managed; }
            set { SetProperty(ref _managed, value); }
        }

        private bool _metadata;
        public bool Metadata
        {
            get { return _metadata; }
            set { SetProperty(ref _metadata, value); }
        }

        private bool _association;
        public bool Association
        {
            get { return _association; }
            set { SetProperty(ref _association, value); }
        }

        public virtual ICollection<Attribute> Attributes { get; set; }

        public virtual ICollection<Relationship> OneToManyRelationships { get; set; }
        public virtual ICollection<Relationship> ManyToOneRelationships { get; set; }

        public virtual ICollection<Proxy> Proxies { get; set; }
    }
}
