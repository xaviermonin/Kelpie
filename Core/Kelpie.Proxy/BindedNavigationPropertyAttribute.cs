using System;

namespace Kelpie.Proxy
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindedNavigationPropertyAttribute : Attribute
    {
        public string NavigationProperty { get; set; }
    }
}
