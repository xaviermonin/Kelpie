using EntityCore.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    class NavigationPropertyBuilder
    {
        private System.Reflection.Emit.TypeBuilder _typeBuilder;

        public NavigationPropertyBuilder(System.Reflection.Emit.TypeBuilder _typeBuilder)
        {
            this._typeBuilder = _typeBuilder;
        }

        internal void CreateNavigationProperties(Models.Entity entity,
                                                 IEnumerable<Type> availablesTypes = null)
        {
            List<MethodInfo> oneToManyGetMethodList = new List<MethodInfo>();

            foreach (var manyToOne in entity.ManyToOneRelationships)
                CreateManyToOneProperty(manyToOne, availablesTypes);

            foreach (var oneToMany in entity.OneToManyRelationships)
                oneToManyGetMethodList.Add(CreateOneToManyProperty(oneToMany, availablesTypes));
        }

        private void CreateManyToOneProperty(Models.Relationship manyToOne,
                                               IEnumerable<Type> availablesTypes = null)
        {
            var type = availablesTypes.Where(c => c.Name == manyToOne.One.Name).Single();
            TypeHelper.CreateProperty(_typeBuilder, manyToOne.OneNavigationName, type);
        }

        private MethodInfo CreateOneToManyProperty(Models.Relationship oneToMany,
                                               IEnumerable<Type> availablesTypes = null)
        {
            var type = availablesTypes.Where(c => c.Name == oneToMany.Many.Name).Single();
            var collectionType = typeof(ICollection<>).MakeGenericType(type);
            PropertyBuilder propBuilder = TypeHelper.CreateProperty(_typeBuilder, oneToMany.ManyNavigationName, collectionType);

            var inverseProxyAttrConstruct = typeof(InversePropertyAttribute).GetConstructor(new Type[] { typeof(string) });
            var inverseProxyBuilder = new CustomAttributeBuilder(inverseProxyAttrConstruct, new string[] { oneToMany.OneNavigationName });
            propBuilder.SetCustomAttribute(inverseProxyBuilder);

            return propBuilder.GetGetMethod();
        }
    }
}
