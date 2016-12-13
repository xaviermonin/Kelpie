using System;
using System.Collections.Generic;

namespace EntityCore.Proxy.Metadata
{
    [BindedEntity(Name = "AttributeType")]
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

        [BindedNavigationProperty]
        ICollection<IAttribute> Attributes { get; }
    }
}
