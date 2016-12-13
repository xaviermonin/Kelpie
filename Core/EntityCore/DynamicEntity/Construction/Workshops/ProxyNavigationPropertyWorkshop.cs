using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using EntityCore.DynamicEntity.Construction.Helpers.Emitter;
using EntityCore.DynamicEntity.Construction.Workshops.Exceptions;
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
    class ProxyNavigationPropertyWorkshop : EntityWorkshop<ProxyNavigationPropertyWorkshop.Result>
    {
        public ProxyNavigationPropertyWorkshop(EntityFactory factory) : base(factory)
        {
        }

        public class Result : EntityWorkshopResult
        {

        }

        protected class NavigationInfo
        {
            public Type NavigationType { get; set; }
            public string PropertyName { get; set; }
            public Type DeclaringType { get; set; }
            public MethodInfo GetMethod { get; set; }
            public MethodInfo SetMethod { get; set; }
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            var navWorkshopResult = Factory.GetEntityWorkshop<NavigationPropertyWorkshop>().GetResults(entity);

            foreach (var proxy in entity.Proxies)
                CreateBindedNavigations(typeBuilder, proxy, navWorkshopResult);

            return new Result();
        }

        /// <summary>
        /// Get the property name of an entity
        /// from BindedNavigationProperty property attribute
        /// </summary>
        /// <param name="proxyNavProp">PropertyInfo</param>
        /// <returns></returns>
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

        /// <summary>
        /// Create one to many navigation.
        /// Find informations and create the many to one proxy property <see cref="Materials.BindingCollection{TOutput, TInput}"/>
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="proxyType"></param>
        /// <param name="proxyNavProp"></param>
        /// <param name="navWorkshopResult"></param>
        private void CreateManyToOneNavigation(TypeBuilder typeBuilder, Type proxyType, PropertyInfo proxyNavProp,
                                               NavigationPropertyWorkshop.Result navWorkshopResult)
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
                GetMethod = navPropertyBuilder.Value.GetMethod,
                SetMethod = navPropertyBuilder.Value.SetMethod
            };

            CreateManyToOneProperty(typeBuilder, proxyNavInfo, entityNavInfo);
        }

        /// <summary>
        /// Create a many to one property.
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="proxyNavInfo"></param>
        /// <param name="entityNavInfo"></param>
        /// <returns></returns>
        private PropertyBuilder CreateManyToOneProperty(TypeBuilder typeBuilder, NavigationInfo proxyNavInfo, NavigationInfo entityNavInfo)
        {
            PropertyBuilder propertyBuilder = PropertyHelper.CreatePropertyExplImpl(typeBuilder, proxyNavInfo.PropertyName,
                                                                                    proxyNavInfo.NavigationType, proxyNavInfo.DeclaringType,
                                                                                    PropertyHelper.PropertyGetSet.Both);

            GenerateManyToOneGetPropertyBody(propertyBuilder.GetMethod as MethodBuilder, entityNavInfo);
            GenerateManyToOneSetPropertyBody(propertyBuilder.SetMethod as MethodBuilder, entityNavInfo);

            return propertyBuilder;
        }

        /// <summary>
        /// Generate the many to one get method body with <see cref="ILGenerator"/>.
        /// </summary>
        /// <param name="getMethod"></param>
        /// <param name="entityNavInfo"></param>
        private void GenerateManyToOneGetPropertyBody(MethodBuilder getMethod, NavigationInfo entityNavInfo)
        {
            var generator = new EmitHelper(getMethod.GetILGenerator());

            generator.ldarg_0()
                     .call(entityNavInfo.GetMethod)
                     .ret();
        }

        /// <summary>
        /// Generate the many to one set method body with <see cref="ILGenerator"/>.
        /// </summary>
        /// <param name="setMethod"></param>
        /// <param name="entityNavInfo"></param>
        private void GenerateManyToOneSetPropertyBody(MethodBuilder setMethod, NavigationInfo entityNavInfo)
        {
            var generator = new EmitHelper(setMethod.GetILGenerator());

            generator.ldarg_0()
                     .ldarg_1()
                     .call(entityNavInfo.SetMethod)
                     .ret();
        }

        #endregion

        #region OneToMany

        /// <summary>
        /// Create a one to many navigation.
        /// Find informations and create the one to many proxy property mapped to the type property of entity.
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="proxyType"></param>
        /// <param name="proxyNavProp"></param>
        /// <param name="navWorkshopResult"></param>
        private void CreateOneToManyNavigation(TypeBuilder typeBuilder, Type proxyType,
                                               PropertyInfo proxyNavProp,
                                               NavigationPropertyWorkshop.Result navWorkshopResult)
        {
            var navPropertyBuilder = navWorkshopResult.OneToMany.Where(c => c.Key.ManyNavigationName == GetEntityPropName(proxyNavProp))
                                                                .SingleOrDefault();

            if (navPropertyBuilder.Equals(default(KeyValuePair<Models.Relationship, PropertyBuilder>)))
                throw new MetaDataException("There are no metadata to describe '" +
                                            GetEntityPropName(proxyNavProp) +
                                            "' one to many relationship as expected in '" +
                                            proxyType.FullName +
                                            "' proxy");

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
                GetMethod = navPropertyBuilder.Value.GetGetMethod(),
                SetMethod = navPropertyBuilder.Value.GetSetMethod()
            };

            CreateOneToManyProperty(typeBuilder, proxyNavInfo, entityNavInfo);
        }

        /// <summary>
        /// Create a one to many property.
        /// This property is mapped to the entity property with a <see cref="Materials.BindingCollection{TOutput, TInput}"/>
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="proxyNavInfo"></param>
        /// <param name="entityNavInfo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generate to one to many get method body with <see cref="ILGenerator"/>.
        /// The IL generated initialize a new <see cref="Materials.BindingCollection{TOutput, TInput}"/>
        /// mapped to the entity property type then returns it.
        /// </summary>
        /// <param name="entityNavInfo"></param>
        /// <param name="iCollectionProxyType"></param>
        /// <param name="propertyBuilder"></param>
        /// <param name="bindingCollectionField"></param>
        /// <param name="bindingCollectionConstructor"></param>
        private void GenerateOneToManyGetPropertyBody(NavigationInfo entityNavInfo, Type iCollectionProxyType,
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
                     .call(entityNavInfo.GetMethod)         // this.NavigationProperty;
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
