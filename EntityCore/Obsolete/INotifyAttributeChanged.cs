using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore
{
    [Obsolete("AttributeChanged semble superflue")]
    public interface INotifyAttributeChanged
    {
        event AttributeChangedEventHandler AttributeChanged;
    }
}
