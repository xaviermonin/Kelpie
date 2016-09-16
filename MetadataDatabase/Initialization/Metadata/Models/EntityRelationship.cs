using EntityCore.Entity;
using System.Collections.Generic;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class EntityRelationship : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EntityRelationship()
        {
            this.Relationships = new HashSet<Relationship>();
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

        public virtual Entity Entity { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
    }
}
