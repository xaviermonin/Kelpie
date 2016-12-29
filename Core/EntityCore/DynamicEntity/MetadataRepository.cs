using EntityCore.Proxy.Metadata;
using System;

namespace EntityCore.DynamicEntity
{
    public class MetadataRepository
    {
        private DynamicEntityContext _context;

        internal MetadataRepository(DynamicEntityContext context)
        {
            _context = context;
        }
        
        public EntityDbSet<IEntity> Entities
        {
            get { return _context.ProxySet<IEntity>(); }
        }

        public EntityDbSet<IAttribute> Attributes
        {
            get { return _context.ProxySet<IAttribute>(); }
        }

        public EntityDbSet<IAttributeType> AttributeTypes
        {
            get { return _context.ProxySet<IAttributeType>(); }
        }

        public EntityDbSet<IProxy> Proxies
        {
            get { return _context.ProxySet<IProxy>(); }
        }

        public EntityDbSet<IListener> Listener
        {
            get { return _context.ProxySet<IListener>(); }
        }

        public void Publish()
        {
            throw new NotImplementedException();
        }
    }
}
