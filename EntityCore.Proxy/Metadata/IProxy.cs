using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.Proxy.Metadata
{
    public interface IProxy : IBaseEntity
    {
        IEntity Entity { get; set; }

        string FullyQualifiedTypeName { get; set; }
    }
}
