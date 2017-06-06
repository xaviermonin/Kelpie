using System;
using System.Collections.Generic;

namespace Kelpie.Proxy.Metadata
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

        int? DefaultLength
        {
            get;
            set;
        }

        [BindedNavigationProperty]
        ICollection<IAttribute> Attributes { get; }
    }
}
