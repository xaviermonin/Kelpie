using EntityCore.Entity;
using System.Data.Entity.Infrastructure;

namespace EntityCore.DynamicEntity.Event
{
    interface IEntityListener
    {
        void OnEntityEvent(EventDispatcher.EntityEvent entityEvent, BaseEntityContext context, DbEntityEntry entity);

        void PostUpdate(DynamicEntityContext context, DbEntityEntry updatedEntries);
        void PostDelete(DynamicEntityContext context, DbEntityEntry deletedEntries);
        void PostCreate(DynamicEntityContext context, DbEntityEntry createdEntries);
        void PreUpdate(DynamicEntityContext context, DbEntityEntry updatingEntries);
        void PreDelete(DynamicEntityContext context, DbEntityEntry deletingEntries);
        void PreCreate(DynamicEntityContext context, DbEntityEntry creatingEntries);
    }
}
