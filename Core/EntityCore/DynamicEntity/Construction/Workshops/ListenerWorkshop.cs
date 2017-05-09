using EntityCore.Entity.Event;
using System;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class ListenerWorkshop : Workshop<ListenerWorkshop.Result>
    {
        public class Result : WorkshopResult
        {
            
        }

        public ListenerWorkshop(EntityFactory factory)
            : base(factory)
        {
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            foreach (var listener in entity.Listeners)
            {
                var listenerType = Type.GetType(listener.FullyQualifiedTypeName);
                EventDispatcher.RegisterEntityChangedListener(typeBuilder, listenerType);
            }

            return new Result();
        }
    }
}
