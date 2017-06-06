using System;

namespace Kelpie.Proxy
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class BindedEntityAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
