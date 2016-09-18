using System;
using System.Collections.Generic;

namespace EntityCore.Proxy.Metadata
{
    public interface IAttributeType : IBaseEntity
    {
        string ClrName
        {
            get;
            set;
        }

        string SqlServerName
        {
            get;
            set;
        }

        Nullable<int> DefaultLength
        {
            get;
            set;
        }

        ICollection<IAttribute> Attributes { get; set; }
    }
}
