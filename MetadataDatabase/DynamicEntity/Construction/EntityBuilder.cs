using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    internal class EntityBuilder
    {
        private ModuleBuilder _moduleBuilder;

        public Models.Entity Entity { get; }
        public TypeBuilder TypeBuilder { get; private set; }

        public bool PropertiesAdded { get; private set; }
        public bool NavigationPropertiesAdded { get; private set; }

        public EntityBuilder(Models.Entity entity, ModuleBuilder moduleBuilder)
        {
            Entity = entity;
            _moduleBuilder = moduleBuilder;
            TypeBuilder = CreateDynamicTypeBuilder<BaseEntity>();
        }

        internal void AddNavigationProperties(IEnumerable<Type> availableTypes = null)
        {
            if (NavigationPropertiesAdded)
                throw new InvalidOperationException("NavigationProperties already added");

            NavigationPropertyBuilder navPropFact = new NavigationPropertyBuilder(TypeBuilder);
            navPropFact.CreateNavigationProperties(Entity.ManyToOneRelationships, Entity.OneToManyRelationships, availableTypes);

            NavigationPropertiesAdded = true;
        }

        internal void AddProperties()
        {
            if (PropertiesAdded)
                throw new InvalidOperationException("Properties already added");

            PropertyFactory propertyFactory = new PropertyFactory(TypeBuilder);
            propertyFactory.CreateProperties(Entity.Attributes);

            PropertiesAdded = true;
        }

        /// <summary>
        /// Exposes a TypeBuilder that can be returned and created outside of the class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private TypeBuilder CreateDynamicTypeBuilder<T>()
            where T : BaseEntity
        {
            Type[] proxies = Entity.Proxies.Select(i => Type.GetType(i.FullyQualifiedTypeName)).ToArray();

            //typeof(T) is for the base class, can be omitted if not needed
            TypeBuilder = _moduleBuilder.DefineType(_moduleBuilder.Name + "." + Entity.Name, TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.Serializable
                                                            | TypeAttributes.BeforeFieldInit, typeof(T), proxies);

            //various class based attributes for WCF and EF
            AddDataContractAttribute();
            AddTableAttribute(Entity.Name);
            //AddDataServiceKeyAttribute();

            return TypeBuilder;
        }

        public void AddDataContractAttribute()
        {
            Type attrType = typeof(DataContractAttribute);
            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
                new object[] { }));
        }

        public void AddTableAttribute(string name)
        {
            Type attrType = typeof(TableAttribute);
            TypeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { name }));
        }
    }
}
