using Kelpie.Proxy;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Kelpie.Entity
{
    public abstract class BaseEntity : IBaseEntity
    {
        [DataMember, Key]
        public int Id { get; set; }

        public T GetAttributeValue<T>(string propertyName)
        {
            var property = GetType().GetProperty(propertyName);
            return (T)property.GetValue(this);
        }

        public void SetAttributeValue<T>(string propertyName, T value)
        {
            var property = GetType().GetProperty(propertyName);
            property.SetValue(this, value);
        }

        public void AddMemberToRelationship(string relationName, IBaseEntity baseEntity)
        {
            throw new NotImplementedException();

            /*var property = GetType().GetProperty(relationName, typeof(void));
            var collection = property.GetMethod.Invoke(this, null);*/
        }
    }
}
