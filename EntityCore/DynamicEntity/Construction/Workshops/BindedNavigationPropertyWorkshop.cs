﻿using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using EntityCore.DynamicEntity.Construction.Helpers.Emitter;
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

        private static string GetEntityPropName(PropertyInfo proxyNavProp)
        {
            return ((BindedNavigationPropertyAttribute)proxyNavProp.GetCustomAttribute(typeof(BindedNavigationPropertyAttribute)))
                                                                   .NavigationProperty ?? proxyNavProp.Name;
        }

        #region ManyToOne

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
            PropertyBuilder propertyBuilder = PropertyHelper.CreatePropertyExplImpl(typeBuilder, proxyNavInfo.PropertyName,
                                                                                    proxyNavInfo.NavigationType, proxyNavInfo.DeclaringType,
                                                                                    PropertyHelper.PropertyGetSet.Both);

            GenerateManyToOneGetPropertyBody(propertyBuilder.GetMethod as MethodBuilder, entityNavInfo);
            GenerateManyToOneSetPropertyBody(propertyBuilder.SetMethod as MethodBuilder, entityNavInfo);

            return propertyBuilder;
        }

        private void GenerateManyToOneGetPropertyBody(MethodBuilder getMethod, NavigationInfo entityNavInfo)
        {
            var generator = new EmitHelper(getMethod.GetILGenerator());

            generator.ldarg_0()
                     .call(entityNavInfo.Method)
                     .ret();
        }

        private void GenerateManyToOneSetPropertyBody(MethodBuilder setMethod, NavigationInfo entityNavInfo)
        {
            var generator = new EmitHelper(setMethod.GetILGenerator());

            generator.ldarg_0()
                     .ldarg_1()
                     .call(entityNavInfo.Method)
                     .ret();
        }

        #endregion

        #region OneToMany

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

        private PropertyBuilder CreateOneToManyProperty(TypeBuilder typeBuilder, NavigationInfo proxyNavInfo, NavigationInfo entityNavInfo)
        {
            var notMappedAttribute = new CustomAttributeBuilder(typeof(NotMappedAttribute).GetConstructor(new Type[] { }), new object[] { });

            // Build BindingCollectionConstructor<TEntityNavType, TProxyNavType>
            var bindingCollectionType = typeof(Materials.BindingCollection<,>).MakeGenericType(entityNavInfo.NavigationType.GetGenericArguments().Single(),
                                                                                               proxyNavInfo.NavigationType.GetGenericArguments().Single());

            PropertyBuilder propertyBuilder = PropertyHelper.CreatePropertyExplImpl(typeBuilder, proxyNavInfo.PropertyName,
                                                                                    proxyNavInfo.NavigationType, proxyNavInfo.DeclaringType,
                                                                                    PropertyHelper.PropertyGetSet.Get);

            // Preparing Reflection instances
            FieldInfo bindingCollectionField = FieldHelper.CreatePrivateField(typeBuilder, bindingCollectionType,
                                                                             "bindingCollection" + proxyNavInfo.PropertyName);

            var ctors = typeof(Materials.BindingCollection<,>).GetConstructors().Single();
            ConstructorInfo bindingCollectionConstructor = TypeBuilder.GetConstructor(bindingCollectionType, ctors);

            GenerateOneToManyGetPropertyBody(entityNavInfo, proxyNavInfo.NavigationType, propertyBuilder,
                                          bindingCollectionField, bindingCollectionConstructor);

            return propertyBuilder;
        }

        private static void GenerateOneToManyGetPropertyBody(NavigationInfo entityNavInfo, Type iCollectionProxyType,
                                                             PropertyBuilder propertyBuilder, FieldInfo bindingCollectionField,
                                                             ConstructorInfo bindingCollectionConstructor)
        {
            // Generate the following code :
            // if (this.bindingCollection == null)
            //    this.bindingCollection = new BindingCollection<Entity, IProxy>(this.NavigationProperty);
            // return this.bindingCollection;

            var generator = new EmitHelper((propertyBuilder.GetMethod as MethodBuilder).GetILGenerator());

            Label afterInitialisationLabel = generator.DefineLabel();

            generator.ldarg_0()
                     .ldfld(bindingCollectionField)
                     .brtrue_s(afterInitialisationLabel)    // if (this.bindingCollection == null) {
                     .ldarg_0()
                     .ldarg_0()
                     .call(entityNavInfo.Method)            // this.NavigationProperty;
                     .newobj(bindingCollectionConstructor)  // new BindingCollection<Entity, IProxy>(<stack>)
                     .stfld(bindingCollectionField)         // this.bindingCollection = <stack>
                     .MarkLabel(afterInitialisationLabel)   // }
                     .ldarg_0()
                     .ldfld(bindingCollectionField)
                     .ret();                                // return this.bindingCollection;
        }

        #endregion
    }
}