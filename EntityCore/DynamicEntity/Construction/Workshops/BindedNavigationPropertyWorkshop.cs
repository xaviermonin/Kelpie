using EntityCore.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class BindedNavigationPropertyWorkshop : EntityWorkshop<BindedNavigationPropertyWorkshop.Result>
    {
        public BindedNavigationPropertyWorkshop(EntityFactory factory) : base(factory)
        {
        }

        public class Result : EntityWorkshopResult
        {

        }

        private class NavigationInfo
        {
            public Type NavigationType { get; set; }
            public string PropertyName { get; set; }
            public Type DeclaringType { get; set; }
            public MethodInfo Method { get; set; }
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            var navWorkshopResult = Factory.GetEntityWorkshop<NavigationPropertyWorkshop>().GetResults(entity);

            foreach (var proxy in entity.Proxies)
                CreateBindedNavigations(typeBuilder, proxy, navWorkshopResult);

            return new Result();
        }

        private void CreateBindedNavigations(TypeBuilder typeBuilder, Models.Proxy proxy,
                                            NavigationPropertyWorkshop.Result navWorkshopResult)
        {
            var proxyType = Type.GetType(proxy.FullyQualifiedTypeName);

            var proxyNavProperties = proxyType.GetProperties()
                                              .Where(prop => Attribute.IsDefined(prop, typeof(BindedNavigationPropertyAttribute)));

            foreach (var proxyNavProp in proxyNavProperties)
            {
                if (proxyNavProp.PropertyType.Name == typeof(ICollection<>).Name)
                    // One to many
                    CreateOneToManyNavigation(typeBuilder, proxyType, proxyNavProp, navWorkshopResult);
                else
                    // Many to one
                    CreateManyToOneNavigation(typeBuilder, proxyType, proxyNavProp, navWorkshopResult);
            }
        }

        private void CreateManyToOneNavigation(TypeBuilder typeBuilder, Type proxyType, PropertyInfo proxyNavProp, NavigationPropertyWorkshop.Result navWorkshopResult)
        {
            var navPropertyBuilder = navWorkshopResult.ManyToOne.Where(c => c.Key.OneNavigationName == GetEntityPropName(proxyNavProp)).Single();

            NavigationInfo proxyNavInfo = new NavigationInfo()
            {
                DeclaringType = proxyType,
                NavigationType = proxyNavProp.PropertyType,
                PropertyName = proxyNavProp.Name
            };

            NavigationInfo entityNavInfo = new NavigationInfo()
            {
                DeclaringType = navPropertyBuilder.Value.DeclaringType,
                NavigationType = navPropertyBuilder.Value.PropertyType,
                PropertyName = GetEntityPropName(proxyNavProp),
                Method = navPropertyBuilder.Value.GetMethod
            };

            CreateManyToOneProperty(typeBuilder, proxyNavInfo, entityNavInfo);
        }

        private PropertyBuilder CreateManyToOneProperty(TypeBuilder typeBuilder, NavigationInfo proxyNavInfo, NavigationInfo entityNavInfo)
        {
            PropertyBuilder propertyBuilder = TypeHelper.CreatePropertyExplImpl(typeBuilder, proxyNavInfo.PropertyName,
                                                                                proxyNavInfo.NavigationType, proxyNavInfo.DeclaringType,
                                                                                TypeHelper.PropertyGetSet.Both);

            GenerateManyToOneGetPropertyBody(propertyBuilder.GetMethod as MethodBuilder, entityNavInfo);
            GenerateManyToOneSetPropertyBody(propertyBuilder.SetMethod as MethodBuilder, entityNavInfo);

            return propertyBuilder;
        }

        private void GenerateManyToOneGetPropertyBody(MethodBuilder getMethod, NavigationInfo entityNavInfo)
        {
            ILGenerator gen = getMethod.GetILGenerator();
            // Preparing locals
            LocalBuilder car = gen.DeclareLocal(getMethod.ReturnType);
            // Preparing labels
            Label label10 = gen.DefineLabel();
            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, entityNavInfo.Method);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, label10);
            gen.MarkLabel(label10);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
        }

        private void GenerateManyToOneSetPropertyBody(MethodBuilder setMethod, NavigationInfo entityNavInfo)
        {
            ILGenerator gen = setMethod.GetILGenerator();
            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, entityNavInfo.Method);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);
        }

        private void CreateOneToManyNavigation(TypeBuilder typeBuilder, Type proxyType,
                                               PropertyInfo proxyNavProp,
                                               NavigationPropertyWorkshop.Result navWorkshopResult)
        {
            var navPropertyBuilder = navWorkshopResult.OneToMany.Where(c => c.Key.ManyNavigationName == GetEntityPropName(proxyNavProp)).Single();

            NavigationInfo proxyNavInfo = new NavigationInfo()
            {
                DeclaringType = proxyType,
                NavigationType = proxyNavProp.PropertyType,
                PropertyName = proxyNavProp.Name
            };

            NavigationInfo entityNavInfo = new NavigationInfo()
            {
                DeclaringType = navPropertyBuilder.Value.DeclaringType,
                NavigationType = navPropertyBuilder.Value.PropertyType,
                PropertyName = GetEntityPropName(proxyNavProp),
                Method = navPropertyBuilder.Value.GetGetMethod()
            };

            CreateOneToManyProperty(typeBuilder, proxyNavInfo, entityNavInfo);
        }

        #region OneToMany

        private IEnumerable<PropertyBuilder> CreateOneToMany(TypeBuilder typeBuilder, Type proxyType,
                                                             IReadOnlyDictionary<Models.Relationship, PropertyBuilder> navProperties)
        {
            var proxyNavProperties = proxyType.GetProperties()
                                              .Where(prop => Attribute.IsDefined(prop, typeof(BindedNavigationPropertyAttribute)));

            foreach (var proxyNavProp in proxyNavProperties)
            {
                var navPropertyBuilder = navProperties.Where(c => c.Key.ManyNavigationName == GetEntityPropName(proxyNavProp))
                                                              .Single();

                NavigationInfo proxyNavInfo = new NavigationInfo()
                {
                    DeclaringType = proxyType,
                    NavigationType = proxyNavProp.PropertyType.GetGenericArguments().Single(),
                    PropertyName = proxyNavProp.Name
                };

                NavigationInfo entityNavInfo = new NavigationInfo()
                {
                    DeclaringType = navPropertyBuilder.Value.DeclaringType,
                    NavigationType = navPropertyBuilder.Value.PropertyType.GetGenericArguments().Single(),
                    PropertyName = GetEntityPropName(proxyNavProp),
                    Method = navPropertyBuilder.Value.GetGetMethod()
                };

                yield return CreateOneToManyProperty(typeBuilder, proxyNavInfo, entityNavInfo);
            }
        }

        private static string GetEntityPropName(PropertyInfo proxyNavProp)
        {
            return ((BindedNavigationPropertyAttribute)proxyNavProp.GetCustomAttribute(typeof(BindedNavigationPropertyAttribute)))
                                                                   .NavigationProperty ?? proxyNavProp.Name;
        }

        private PropertyBuilder CreateOneToManyProperty(TypeBuilder typeBuilder, NavigationInfo proxyNavInfo, NavigationInfo entityNavInfo)
        {
            var notMappedAttribute = new CustomAttributeBuilder(typeof(NotMappedAttribute).GetConstructor(new Type[] { }), new object[] { });

            // Build BindingCollectionConstructor<TEntityNavType, TProxyNavType>
            var bindingCollectionType = typeof(Materials.BindingCollection<,>).MakeGenericType(entityNavInfo.NavigationType.GetGenericArguments().Single(),
                                                                                               proxyNavInfo.NavigationType.GetGenericArguments().Single());

            PropertyBuilder propertyBuilder = TypeHelper.CreatePropertyExplImpl(typeBuilder, proxyNavInfo.PropertyName,
                                                                                proxyNavInfo.NavigationType, proxyNavInfo.DeclaringType,
                                                                                TypeHelper.PropertyGetSet.Get);

            // Preparing Reflection instances
            FieldInfo bindingCollectionField = TypeHelper.CreatePrivateField(typeBuilder, bindingCollectionType,
                                                                             "bindingCollection" + proxyNavInfo.PropertyName);

            var ctors = typeof(Materials.BindingCollection<,>).GetConstructors().Single();
            ConstructorInfo bindingCollectionConstructor = TypeBuilder.GetConstructor(bindingCollectionType, ctors);

            GenerateOneToManyPropertyBody(entityNavInfo, proxyNavInfo.NavigationType, propertyBuilder,
                                                  bindingCollectionField, bindingCollectionConstructor);

            return propertyBuilder;
        }

        private static void GenerateOneToManyPropertyBody(NavigationInfo entityNavInfo, Type iCollectionProxyType,
                                         PropertyBuilder propertyBuilder, FieldInfo bindingCollectionField,
                                         ConstructorInfo bindingCollectionConstructor)
        {
            // Adding parameters
            ILGenerator gen = (propertyBuilder.GetMethod as MethodBuilder).GetILGenerator();
            // Preparing locals
            LocalBuilder flag = gen.DeclareLocal(typeof(Boolean));
            LocalBuilder is2 = gen.DeclareLocal(iCollectionProxyType);
            // Preparing labels
            Label label31 = gen.DefineLabel();
            Label label40 = gen.DefineLabel();

            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, bindingCollectionField);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Brfalse_S, label31);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, entityNavInfo.Method);
            gen.Emit(OpCodes.Newobj, bindingCollectionConstructor);
            gen.Emit(OpCodes.Stfld, bindingCollectionField);
            gen.MarkLabel(label31);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, bindingCollectionField);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br_S, label40);
            gen.MarkLabel(label40);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ret);
        }

        #endregion
    }
}
