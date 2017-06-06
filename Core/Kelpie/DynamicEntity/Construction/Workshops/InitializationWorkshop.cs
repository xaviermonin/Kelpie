using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Models = Kelpie.Initialization.Metadata.Models;

namespace Kelpie.DynamicEntity.Construction.Workshops
{
    class InitializationWorkshop : Workshop<InitializationWorkshop.Result>
    {
        public class Result : WorkshopResult
        {
            public IEnumerable<Type> Proxies { get; internal set; }
        }

        public InitializationWorkshop(EntityFactory factory)
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
