using System;

namespace EntityCore.Proxy
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindedNavigationPropertyAttribute : Attribute
    {
        public string NavigationProperty { get; set; }
    }
}
