using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace EntityCore.DynamicEntity.Event
{
    class EventDispatcher
    {
        public enum EntityEvent { PreCreate, PostCreate, PreUpdate, PostUpdate, PreDelete, PostDelete }
        static private List<KeyValuePair<string, IEntityListener>> entitiesChangedListeners = new List<KeyValuePair<string, IEntityListener>>();

        public static void RegisterEntityChangedListener(string entityName, EntityEvent entityEvent, IEntityListener entityChanged)
        {
            if (entityChanged == null)
                throw new ArgumentNullException("entityChanged", "Behavior is null");

            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException("entityName", "Entity name invalid");

            if (entitiesChangedListeners.Any(c => c.Key == entityName && c.Value == entityChanged))
                throw new InvalidOperationException("This event listener is already attach to this entity: " + entityName);

            entitiesChangedListeners.Add(new KeyValuePair<string, IEntityListener>(entityName, entityChanged));
        }

        internal static void DispatchEvent(EntityEvent entityEvent, string entityName, BaseEntityContext context, DbEntityEntry entry)
        {
            foreach (var eventListener in entitiesChangedListeners.Where(c => c.Key == entityName))
                eventListener.Value.OnEntityEvent(entityEvent, context, entry);
        }
    }
}
