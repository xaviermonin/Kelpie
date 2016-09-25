using EntityCore.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        private string _oneNavigationName;
        public string OneNavigationName
        {
            get { return _oneNavigationName; }
            set { SetProperty(ref _oneNavigationName, value); }
        }

        private string _manyNavigationName;
        public string ManyNavigationName
        {
            get { return _manyNavigationName; }
            set { SetProperty(ref _manyNavigationName, value); }
        }

        [InverseProperty("OneToManyRelationships")]
        public virtual Entity One { get; set; }
        [InverseProperty("ManyToOneRelationships")]
        public virtual Entity Many { get; set; }
    }
}
