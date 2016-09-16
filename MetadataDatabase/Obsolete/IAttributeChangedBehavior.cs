using EntityCore.Entity;
using System;

namespace EntityCore
{
    public delegate void AttributeChangedEventHandler(BaseEntity baseEntity, AttributeChangedEventArgs args);

    [Obsolete("AttributeChanged semble superflue")]
    public interface IAttributeChangedBehavior
    {
        void OnPropertyChanged(BaseEntity entity, string propertyName);
    }
}
