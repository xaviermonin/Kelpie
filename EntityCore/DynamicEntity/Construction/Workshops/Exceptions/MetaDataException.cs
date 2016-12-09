using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity.Construction.Workshops.Exceptions
{
    [Serializable]
    public class MetaDataException : Exception
    {
        public MetaDataException() { }
        public MetaDataException(string message) : base(message) { }
        public MetaDataException(string message, Exception inner) : base(message, inner) { }
        protected MetaDataException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
