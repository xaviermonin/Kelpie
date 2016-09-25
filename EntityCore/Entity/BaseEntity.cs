using EntityCore.Proxy;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace EntityCore.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        private int _id;

        [DataMember, Key]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            T oldValue = storage;

            storage = value;
            
            return true;
        }

        public T GetAttributeValue<T>(string propertyName)
        {
            var property = this.GetType().GetProperty(propertyName);
            return (T)property.GetValue(this);
        }

        public void SetAttributeValue<T>(string propertyName, T value)
        {
            var property = this.GetType().GetProperty(propertyName);
            property.SetValue(this, value);
        }
    }
}
