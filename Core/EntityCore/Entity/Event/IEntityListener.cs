using System.Data.Entity.Infrastructure;

namespace EntityCore.Entity.Event
{
    interface IEntityListener
    {
        void OnEntityEvent(EventDispatcher.EntityEvent entityEvent, BaseEntityContext context, DbEntityEntry entity);
    }
}
