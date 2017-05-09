using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using EntityCore.DynamicEntity.Construction.Helpers.Emitter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class NavigationPropertyWorkshop : Workshop<NavigationPropertyWorkshop.Result>
    {
        public class Result : WorkshopResult
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

            var fieldInfo = FieldHelper.CreatePrivateField(typeBuilder, collectionType,
                                                           $"__navProp_{oneToMany.Many.Name}");

            var propertyBuilder = PropertyHelper.CreateProperty(typeBuilder, oneToMany.ManyNavigationName,
                                                                collectionType, PropertyHelper.PropertyGetSet.Both);

            var hashSetType = typeof(HashSet<>).MakeGenericType(type);

            var ctors = typeof(HashSet<>).GetConstructor(Type.EmptyTypes);
            var hashSetConstructor = TypeBuilder.GetConstructor(hashSetType, ctors);

            GenerateOneToManyGetPropertyBody(propertyBuilder.GetMethod as MethodBuilder, fieldInfo, hashSetConstructor);
            GenerateOneToManySetPropertyBody(propertyBuilder.SetMethod as MethodBuilder, fieldInfo);

            // Apply InverseProperty Attribute
            var inverseProxyAttrConstruct = typeof(InversePropertyAttribute).GetConstructor(new Type[] { typeof(string) });
            var inverseProxyBuilder = new CustomAttributeBuilder(inverseProxyAttrConstruct, new string[] { oneToMany.OneNavigationName });
            propertyBuilder.SetCustomAttribute(inverseProxyBuilder);

            return new KeyValuePair<Models.Relationship, PropertyBuilder>(oneToMany, propertyBuilder);
        }

        private void GenerateOneToManySetPropertyBody(MethodBuilder methodBuilder, FieldBuilder fieldInfo)
        {
            var generator = new EmitHelper(methodBuilder.GetILGenerator());
            generator.ldarg_0()
                     .ldarg_1()
                     .stfld(fieldInfo)
                     .ret();
        }

        private void GenerateOneToManyGetPropertyBody(MethodBuilder methodBuilder, FieldInfo fieldInfo,
                                                      ConstructorInfo hashSetConstructor)
        {
            var generator = new EmitHelper(methodBuilder.GetILGenerator());

            Label afterInitialization = generator.DefineLabel();

            generator.ldarg_0()
                     .ldfld(fieldInfo)
                     .brtrue_s(afterInitialization)
                     .ldarg_0()
                     .newobj(hashSetConstructor)
                     .stfld(fieldInfo)
                     .MarkLabel(afterInitialization)
                     .ldarg_0()
                     .ldfld(fieldInfo)
                     .ret();
        }
    }
}
