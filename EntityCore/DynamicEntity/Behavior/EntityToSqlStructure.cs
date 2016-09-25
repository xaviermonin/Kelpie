using EntityCore.DynamicEntity.Event;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity.Behavior
{
    class EntityToSqlStructure : IEntityListener
    {
        public void OnEntityEvent(EventDispatcher.EntityEvent entityEvent, Entity.BaseEntityContext context, DbEntityEntry entity)
        {
            
        }

        public void PostUpdate(DynamicEntityContext context, DbEntityEntry updatedEntries)
        {
            
        }

        public void PostDelete(DynamicEntityContext context, DbEntityEntry deletedEntries)
        {
            
        }

        public void PostCreate(DynamicEntityContext context, DbEntityEntry createdEntries)
        {
            
        }

        public void PreUpdate(DynamicEntityContext context, DbEntityEntry updatingEntries)
        {
            
        }

        public void PreDelete(DynamicEntityContext context, DbEntityEntry deletingEntries)
        {
            
        }

        public void PreCreate(DynamicEntityContext context, DbEntityEntry creatingEntries)
        {
            
        }
    }
}
