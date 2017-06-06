using Kelpie.DynamicEntity.Construction.Helper.Reflection;
using Kelpie.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Kelpie.Entity.Event
{
    class EventDispatcher
    {
        public enum EntityEvent { PreCreate, PostCreate, PreUpdate, PostUpdate, PreDelete, PostDelete }
        static private List<KeyValuePair<Type, Type>> entitiesChangedListeners = new List<KeyValuePair<Type, Type>>();

        public static void RegisterEntityChangedListener<TEntityListener>(Type entityType)
            where TEntityListener : IEntityListener
        {
            var entityListenerType = typeof(TEntityListener);

            RegisterEntityChangedListener(entityType, entityListenerType);
        }

        public static void RegisterEntityChangedListener(Type entityType, Type listenerType)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType", "Entity type is null");

            if (listenerType.IsSubclassOf(typeof(IEntityListener)))
                throw new ArgumentException("listenerType", "Invalid Listener");

            if (entitiesChangedListeners.Any(c => c.Key == entityType && c.Value == listenerType))
                throw new InvalidOperationException("This event listener is already attach to this entity: " + entityType.Name);

            entitiesChangedListeners.Add(new KeyValuePair<Type, Type>(entityType, listenerType));
        }

        internal static void DispatchEvent(EntityEvent entityEvent, Type entityType,
                                           BaseEntityContext context, DbEntityEntry entry)
        {
            foreach (var eventListener in entitiesChangedListeners.Where(c => TypeHelper.IsSameOrSubclass(c.Key, entityType)))
            {
                IEntityListener listener = (IEntityListener)Activator.CreateInstance(eventListener.Value, false);
                listener.OnEntityEvent(entityEvent, context, entry);
            }
        }
    }
}
