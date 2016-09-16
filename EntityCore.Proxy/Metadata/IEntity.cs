
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

        bool Managed
        {
            get;
            set;
        }

        ICollection<IAttribute> Attributes { get; set; }
        ICollection<IInterface> Interfaces { get; set; }
    }
}
