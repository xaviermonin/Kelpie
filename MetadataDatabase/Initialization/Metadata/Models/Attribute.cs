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

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _isNullable;
        public bool IsNullable
        {
            get { return _isNullable; }
            set { SetProperty(ref _isNullable, value); }
        }

        private string _defaultValue;
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { SetProperty(ref _defaultValue, value); }
        }

        private Nullable<int> _length;
        public Nullable<int> Length
        {
            get { return _length; }
            set { SetProperty(ref _length, value); }
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
            get { return _managed; }
            set { SetProperty(ref _managed, value); }
        }

        public virtual AttributeType Type { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
