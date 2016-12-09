
using System.Collections.Generic;
namespace EntityCore.Proxy.Metadata
{
    public interface IEntity : IBaseEntity
    {
        string Name
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        bool? Managed
        {
            get;
            set;
        }

        [BindedNavigationProperty]
        ICollection<IAttribute> Attributes { get; }

        [BindedNavigationProperty]
        ICollection<IProxy> Proxies { get; }
    }
}
