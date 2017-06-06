using Kelpie.Proxy.Metadata;
using System;

namespace Kelpie.DynamicEntity
{
    public class MetadataRepository
    {
        private DynamicEntityContext _context;

        internal MetadataRepository(DynamicEntityContext context)
        {
            _context = context;
        }
        
        public ProxyDbSet<IEntity> Entities
        {
            get { return _context.ProxySet<IEntity>(); }
        }

        public ProxyDbSet<IAttribute> Attributes
        {
            get { return _context.ProxySet<IAttribute>(); }
        }

        public ProxyDbSet<IAttributeType> AttributeTypes
        {
            get { return _context.ProxySet<IAttributeType>(); }
        }

        public ProxyDbSet<IProxy> Proxies
        {
            get { return _context.ProxySet<IProxy>(); }
        }

        public ProxyDbSet<IListener> Listener
        {
            get { return _context.ProxySet<IListener>(); }
        }

        public void Publish()
        {
            throw new NotImplementedException();
        }
    }
}
