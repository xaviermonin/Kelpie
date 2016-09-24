using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Proxy
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindedNavigationPropertyAttribute : Attribute
    {
        public string NavigationProperty { get; set; }
    }
}
