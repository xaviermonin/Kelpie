using System;

namespace Kelpie.DynamicEntity.Construction.Workshops.Exceptions
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
