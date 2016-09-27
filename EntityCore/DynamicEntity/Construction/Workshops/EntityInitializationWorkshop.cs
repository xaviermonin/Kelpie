using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityCore.Initialization.Metadata.Models;
using Models = EntityCore.Initialization.Metadata.Models;
using System.Reflection.Emit;
using EntityCore.Entity;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class EntityInitializationWorkshop : EntityWorkshop<EntityInitializationWorkshop.Result>
    {
        public class Result : EntityWorkshopResult
        {
            public IEnumerable<Type> Proxies { get; internal set; }
        }

        public EntityInitializationWorkshop(EntityFactory factory)
            : base(factory)
        {
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            AddDataContractAttribute(typeBuilder);
            AddTableAttribute(entity.Name, typeBuilder);

            Type[] proxies = entity.Proxies.Select(i => Type.GetType(i.FullyQualifiedTypeName)).ToArray();

            foreach (var proxy in proxies)
                typeBuilder.AddInterfaceImplementation(proxy);

            return new Result() {
                Proxies = proxies
            };
        }

        public void AddDataContractAttribute(TypeBuilder typeBuilder)
        {
            Type attrType = typeof(DataContractAttribute);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
                new object[] { }));
        }

        public void AddTableAttribute(string name, TypeBuilder typeBuilder)
        {
            Type attrType = typeof(TableAttribute);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { name }));
        }
    }
}
