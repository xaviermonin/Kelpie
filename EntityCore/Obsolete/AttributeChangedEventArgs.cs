using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityCore
{
    [Obsolete("AttributeChanged semble superflue")]
    public class AttributeChangedEventArgs
    {
        public AttributeChangedEventArgs(string propertyName, object oldValue)
        {
            this.PropertyName = propertyName;
            this.OldValue = oldValue;
        }

        public Object OldValue { get; set; }
        public string PropertyName { get; set; }
    }
}
