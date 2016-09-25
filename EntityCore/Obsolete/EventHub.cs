using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore
{
    [Obsolete("AttributeChanged semble superflue")]
    class EventHub
    {
        private static List<KeyValuePair<string, IAttributeChangedBehavior>> attributeChangedBehavior = new List<KeyValuePair<string, IAttributeChangedBehavior>>();

        public static void RegisterAttributeChanged(string entityName, IAttributeChangedBehavior behavior)
        {
            if (behavior == null)
                throw new ArgumentNullException("behavior", "Behavior is null");

            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException("entityName", "Entity name invalid");

            if (attributeChangedBehavior.Any(c => c.Key == entityName && c.Value == behavior))
                throw new InvalidOperationException("This behavior is already attach to this entity: " + entityName);

            attributeChangedBehavior.Add(new KeyValuePair<string, IAttributeChangedBehavior>(entityName, behavior));
        }

        private static void OnAttributeChanged(BaseEntity baseEntity, AttributeChangedEventArgs args)
        {
            var behaviors = attributeChangedBehavior.Where(c => c.Key == baseEntity.GetType().Name);
            foreach (var behavior in behaviors)
                behavior.Value.OnPropertyChanged(baseEntity, args.PropertyName);
        }

        internal static void SubscribeEntity(BaseEntity baseEntity)
        {
            baseEntity.AttributeChanged += OnAttributeChanged;
        }
    }
}
