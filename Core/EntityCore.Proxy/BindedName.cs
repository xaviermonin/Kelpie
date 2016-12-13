using System;

namespace EntityCore.Proxy
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class BindedEntityAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
