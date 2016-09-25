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
    class BindedNavigationPropertyBuilder
    {
        private System.Reflection.Emit.TypeBuilder _typeBuilder;

        public BindedNavigationPropertyBuilder(System.Reflection.Emit.TypeBuilder _typeBuilder)
        {
            this._typeBuilder = _typeBuilder;
        }

        internal void CreateBindedNavigationProperty(Models.Entity entity)
        {
            foreach (var proxy in entity.Proxies)
                CreateBindedNavigationProperty(proxy, null /* Method info */);
        }

        private void CreateBindedNavigationProperty(Models.Proxy proxy, IEnumerable<MethodInfo> getMethodsInfos)
        {
            var proxyType = Type.GetType(proxy.FullyQualifiedTypeName);

            var proxyNavProperties = proxyType.GetProperties()
                                              .Where(prop => Attribute.IsDefined(prop, typeof(BindedNavigationPropertyAttribute)));
            
            foreach (var navProp in proxyNavProperties)
            {
                var proxyNavPropType = navProp.PropertyType;
                var proxyNavPropEncapsulatedType = proxyNavPropType.GetGenericArguments().Single();
                var bindedNavPropAttribute = (BindedNavigationPropertyAttribute)navProp.GetCustomAttribute(typeof(BindedNavigationPropertyAttribute));
                var propertyName = bindedNavPropAttribute.NavigationProperty ?? navProp.Name;
                var getPropertyMethod = getMethodsInfos.Where(m => m.Name == "get_" + propertyName).Single();
                var bindedNavPropEncapsulatedType = getPropertyMethod.ReturnType.GetGenericArguments().Single();

                CreateProxyNavigationBinding(proxyType, proxyNavPropEncapsulatedType, propertyName, getPropertyMethod, bindedNavPropEncapsulatedType);
            }
        }

        private PropertyBuilder CreateProxyNavigationBinding(Type proxyType, Type proxyNavType, string propertyName, MethodInfo getPropertyMethod, Type bindedNavType)
        {
            var notMappedAttributeConstructor = typeof(NotMappedAttribute).GetConstructor(new Type[] { });
            var customerAttributeBuilder = new CustomAttributeBuilder(notMappedAttributeConstructor, new object[] { });
            // Attribute | IAttribute
            var bindingCollectionType = typeof(Materials.BindingCollection<,>).MakeGenericType(bindedNavType, proxyNavType);
            var iCollectionProxyType = typeof(ICollection<>).MakeGenericType(proxyNavType);
            var iCollectionBindedType = typeof(ICollection<>).MakeGenericType(bindedNavType);

            var getProxyMethod = TypeHelper.CreateGetExplImplMethod(_typeBuilder, propertyName, iCollectionProxyType, proxyType);
            /* new CustomAttributeBuilder[] { customerAttributeBuilder } );*/

            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(string.Format("{0}.{1}", proxyType.FullName, propertyName),
                                                                          PropertyAttributes.HasDefault, iCollectionProxyType, null);

            // Preparing Reflection instances
            FieldInfo bindingCollectionField = TypeHelper.CreatePrivateField(_typeBuilder, bindingCollectionType, "bindingCollection" + propertyName);

            var ctors = typeof(Materials.BindingCollection<,>).GetConstructors().Single();
            ConstructorInfo bindingCollectionConstructor = TypeBuilder.GetConstructor(bindingCollectionType, ctors);

            // Setting return type
            getProxyMethod.SetReturnType(iCollectionProxyType);
            // Adding parameters
            ILGenerator gen = getProxyMethod.GetILGenerator();
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
            gen.Emit(OpCodes.Call, getPropertyMethod);
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

            propertyBuilder.SetGetMethod(getProxyMethod);

            return propertyBuilder;
        }
    }
}
