using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class NavigationPropertyWorkshop : EntityWorkshop<NavigationPropertyWorkshop.Result>
    {
        public class Result : EntityWorkshopResult
        {
            public IReadOnlyDictionary<Models.Relationship, PropertyBuilder> OneToMany { get; internal set; }
            public IReadOnlyDictionary<Models.Relationship, PropertyBuilder> ManyToOne { get; internal set; }
        }

        public NavigationPropertyWorkshop(EntityFactory factory)
            : base(factory)
        {
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            IEnumerable<Type> knowEntities = Factory.JobBags.Select(jb => jb.Type);

            var manyToOneResult = entity.ManyToOneRelationships.Select(r => CreateManyToOneProperty(r, typeBuilder, knowEntities));
            var oneToManyResult = entity.OneToManyRelationships.Select(r => CreateOneToManyProperty(r, typeBuilder, knowEntities));

            return new Result
            {
                ManyToOne = manyToOneResult.ToDictionary(r => r.Key, r => r.Value),
                OneToMany = oneToManyResult.ToDictionary(r => r.Key, r => r.Value),
            };
        }

        private KeyValuePair<Models.Relationship, PropertyBuilder> CreateManyToOneProperty(Models.Relationship manyToOne,
                                                                                           TypeBuilder typeBuilder,
                                                                                           IEnumerable<Type> knowEntities)
        {
            Type type = knowEntities.Where(c => c.Name == manyToOne.One.Name).Single();
            PropertyBuilder propertyBuilder = PropertyHelper.CreateAutoProperty(typeBuilder, manyToOne.OneNavigationName,
                                                                                type, PropertyHelper.PropertyGetSet.Both);
            return new KeyValuePair<Models.Relationship, PropertyBuilder>(manyToOne, propertyBuilder);
        }
        
        private KeyValuePair<Models.Relationship, PropertyBuilder> CreateOneToManyProperty(Models.Relationship oneToMany,
                                                                                           TypeBuilder typeBuilder,
                                                                                           IEnumerable<Type> knowEntities)
        {
            var type = knowEntities.Where(c => c.Name == oneToMany.Many.Name).Single();
            var collectionType = typeof(ICollection<>).MakeGenericType(type);
            PropertyBuilder propertyBuilder = PropertyHelper.CreateAutoProperty(typeBuilder, oneToMany.ManyNavigationName,
                                                                                collectionType, PropertyHelper.PropertyGetSet.Both);

            var inverseProxyAttrConstruct = typeof(InversePropertyAttribute).GetConstructor(new Type[] { typeof(string) });
            var inverseProxyBuilder = new CustomAttributeBuilder(inverseProxyAttrConstruct, new string[] { oneToMany.OneNavigationName });
            propertyBuilder.SetCustomAttribute(inverseProxyBuilder);

            return new KeyValuePair<Models.Relationship, PropertyBuilder>(oneToMany, propertyBuilder);
        }
    }
}
