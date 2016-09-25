using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Proxy.Metadata
{
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

        Nullable<int> Length
        {
            get;
            set;
        }

        bool? Managed
        {
            get;
            set;
        }

        /*IAttributeType Type { get; set; }
        IEntity Entity { get; set; }*/
    }
}
