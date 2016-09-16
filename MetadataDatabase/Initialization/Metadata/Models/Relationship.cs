using EntityCore.Entity;
using System.Collections.Generic;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class Relationship : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Relationship()
        {
            
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

        private int _relationshipType;
        public int RelationshipType
        {
            get { return _relationshipType; }
            set { SetProperty(ref _relationshipType, value); }
        }

        private int _cascadeDelete;
        public int CascadeDelete
        {
            get { return _cascadeDelete; }
            set { SetProperty(ref _cascadeDelete, value); }
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

        public virtual EntityRelationship ReferencedEntity { get; set; }
        public virtual EntityRelationship ReferencingEntity { get; set; }
    }
}
