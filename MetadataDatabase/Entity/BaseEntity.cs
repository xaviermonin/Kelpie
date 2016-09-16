using EntityCore.Proxy;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace EntityCore.Entity
{
    public abstract class BaseEntity : INotifyAttributeChanged, IBaseEntity
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

        protected void NotifyEventHub()
        {
            EventHub.SubscribeEntity(this);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            T oldValue = storage;

            storage = value;
            this.OnPropertyChanged(oldValue, propertyName);
            return true;
        }

        public event AttributeChangedEventHandler AttributeChanged;

        protected void OnPropertyChanged<T>(T oldValue, [CallerMemberName] string propertyName = null)
        {
            if (AttributeChanged != null)
                AttributeChanged.Invoke(this, new AttributeChangedEventArgs(propertyName, oldValue));
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
