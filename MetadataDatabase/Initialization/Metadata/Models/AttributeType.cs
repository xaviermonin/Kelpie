using EntityCore.Entity;
using System;
using System.Collections.Generic;

namespace EntityCore.Initialization.Metadata.Models
{
    internal class AttributeType : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AttributeType()
        {
            this.Attributes = new HashSet<Attribute>();
        }

        private string _clrName;
        public string ClrName
        {
            get { return _clrName; }
            set { SetProperty(ref _clrName, value); }
        }

        private string _sqlServerName;
        public string SqlServerName
        {
            get { return _sqlServerName; }
            set { SetProperty(ref _sqlServerName, value); }
        }

        private Nullable<int> _defaultLength;
        public Nullable<int> DefaultLength
        {
            get { return _defaultLength; }
            set { SetProperty(ref _defaultLength, value); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attribute> Attributes { get; set; }
    }
}
