using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Proxy
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class BindedEntityAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
