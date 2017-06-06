using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataModel.DynamicEntity
{
    static class EventDispatcher
    {
        public enum EntityEvent { PreCreate, PostCreate, PreUpdate, PostUpdate, PreDelete, PostDelete }
        static private List<KeyValuePair<string, IEntityChanged>> entitiesChangedListeners = new List<KeyValuePair<string, IEntityChanged>>();

        public static void RegisterEntityChangedListener(string entityName, EntityEvent entityEvent, IEntityChanged entityChanged)
        {
            if (entityChanged == null)
                throw new ArgumentNullException("entityChanged", "Behavior is null");

            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException("entityName", "Entity name invalid");

            if (entitiesChangedListeners.Any(c => c.Key == entityName && c.Value == entityChanged))
                throw new InvalidOperationException("This event listener is already attach to this entity: " + entityName);

            entitiesChangedListeners.Add(new KeyValuePair<string, IEntityChanged>(entityName, entityChanged));
        }

        public static void DispatchEvent(EntityEvent entityEvent, string entityName)
        {
            foreach (var eventListener in entitiesChangedListeners.Where(c => c.Key == entityName))
                eventListener.Value.OnEntityChanged();
        }
    }
}
