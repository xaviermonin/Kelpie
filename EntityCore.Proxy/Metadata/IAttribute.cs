using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Proxy.Metadata
{
    [BindedEntity(Name = "Attribute")]
    public interface IAttribute : IBaseEntity
    {
        string Name
        {
            get;
            set;
        }

        bool? IsNullable
        {
            get;
            set;
        }

        string DefaultValue
        {
            get;
            set;
        }

        int? Length
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
        IAttributeType Type { get; set; }

        [BindedNavigationProperty]
        IEntity Entity { get; set; }
    }
}
