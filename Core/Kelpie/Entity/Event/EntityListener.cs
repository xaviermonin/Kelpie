using System.Data.Entity.Infrastructure;

namespace Kelpie.Entity.Event
{
    abstract class EntityListener : IEntityListener
    {
        public void OnEntityEvent(EventDispatcher.EntityEvent entityEvent, BaseEntityContext context, DbEntityEntry entity)
        {
            switch (entityEvent)
            {
                case EventDispatcher.EntityEvent.PostUpdate:    PreUpdate(context, entity); break;
                case EventDispatcher.EntityEvent.PostDelete:    PostDelete(context, entity); break;
                case EventDispatcher.EntityEvent.PostCreate:    PostCreate(context, entity); break;
                case EventDispatcher.EntityEvent.PreUpdate:     PreUpdate(context, entity); break;
                case EventDispatcher.EntityEvent.PreDelete:     PreDelete(context, entity); break;
                case EventDispatcher.EntityEvent.PreCreate:     PreCreate(context, entity); break;
            }
        }

        public abstract void PostUpdate(BaseEntityContext context, DbEntityEntry updatedEntries);
        public abstract void PostDelete(BaseEntityContext context, DbEntityEntry deletedEntries);
        public abstract void PostCreate(BaseEntityContext context, DbEntityEntry createdEntries);
        public abstract void PreUpdate(BaseEntityContext context, DbEntityEntry updatingEntries);
        public abstract void PreDelete(BaseEntityContext context, DbEntityEntry deletingEntries);
        public abstract void PreCreate(BaseEntityContext context, DbEntityEntry creatingEntries);
    }
}
