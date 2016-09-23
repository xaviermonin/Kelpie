using System;
using System.Collections.Generic;
using System.Linq;
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

        internal void CreateNavigationProperties(IEnumerable<Models.Relationship> oneToManyRelationship,
                                                 IEnumerable<Models.Relationship> manyToOneRelationship,
                                                 IEnumerable<Type> availablesTypes = null)
        {
            foreach (var manyToOne in manyToOneRelationship)
            {
                var type = availablesTypes.Where(c => c.Name == manyToOne.One.Name).Single();
                TypeHelper.CreatePropertyForType(_typeBuilder, manyToOne.OneNavigationName, type);
            }

            foreach (var oneToMany in oneToManyRelationship)
            {
                var type = availablesTypes.Where(c => c.Name == oneToMany.Many.Name).Single();
                var collectionType = typeof(ICollection<>).MakeGenericType(type);
                TypeHelper.CreatePropertyForType(_typeBuilder, oneToMany.ManyNavigationName, collectionType);
            }
        }
    }
}
