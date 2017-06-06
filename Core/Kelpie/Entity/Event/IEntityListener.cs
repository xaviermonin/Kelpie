using System.Data.Entity.Infrastructure;

namespace Kelpie.Entity.Event
{
    interface IEntityListener
    {
        void OnEntityEvent(EventDispatcher.EntityEvent entityEvent, BaseEntityContext context, DbEntityEntry entity);
    }
}
