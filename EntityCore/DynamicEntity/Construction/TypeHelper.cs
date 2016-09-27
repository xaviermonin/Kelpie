using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity.Construction
{
    static class TypeHelper
    {
        /// <summary>
        /// [ <c>public static Type GetNullableType(Type TypeToConvert)</c> ]
        /// <para></para>
        /// Convert any Type to its Nullable&lt;T&gt; form, if possible
        /// </summary>
        /// <param name="typeToConvert">The Type to convert</param>
        /// <returns>
        /// The Nullable&lt;T&gt; converted from the original type, the original type if it was already nullable, or null 
        /// if either <paramref name="typeToConvert"/> could not be converted or if it was null.
        /// </returns>
        /// <remarks>
        /// To qualify to be converted to a nullable form, <paramref name="typeToConvert"/> must contain a non-nullable value 
        /// type other than System.Void.  Otherwise, this method will return a null.
        /// </remarks>
        /// <seealso cref="Nullable&lt;T&gt;"/>
        public static Type GetNullableType(Type typeToConvert)
        {
            // Abort if no type supplied
            if (typeToConvert == null)
                return null;

            // If the given type is already nullable, just return it
            if (IsTypeNullable(typeToConvert))
                return typeToConvert;

            // If the type is a ValueType and is not System.Void, convert it to a Nullable<Type>
            if (typeToConvert.IsValueType && typeToConvert != typeof(void))
                return typeof(Nullable<>).MakeGenericType(typeToConvert);

            // Done - no conversion
            return null;
        }

        /// <summary>
        /// [ <c>public static bool IsTypeNullable(Type TypeToTest)</c> ]
        /// <para></para>
        /// Reports whether a given Type is nullable (Nullable&lt; Type &gt;)
        /// </summary>
        /// <param name="typeToTest">The Type to test</param>
        /// <returns>
        /// true = The given Type is a Nullable&lt; Type &gt;; false = The type is not nullable, or <paramref name="typeToTest"/> 
        /// is null.
        /// </returns>
        /// <remarks>
        /// This method tests <paramref name="typeToTest"/> and reports whether it is nullable (i.e. whether it is either a 
        /// reference type or a form of the generic Nullable&lt; T &gt; type).
        /// </remarks>
        /// <seealso cref="GetNullableType"/>
        public static bool IsTypeNullable(Type typeToTest)
        {
            // Abort if no type supplied
            if (typeToTest == null)
                return false;

            // If this is not a value type, it is a reference type, so it is automatically nullable
            //  (NOTE: All forms of Nullable<T> are value types)
            if (!typeToTest.IsValueType)
                return true;

            // Report whether TypeToTest is a form of the Nullable<> type
            return typeToTest.IsGenericType && typeToTest.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        static public PropertyBuilder CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = TypeHelper.CreatePrivateField(typeBuilder,propertyType, propertyName);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            //creates the Get Method for the property
            propertyBuilder.SetGetMethod(CreateGetMethod(typeBuilder, getterAndSetterAttributes, propertyName, propertyType, fieldBuilder));
            //creates the Set Method for the property and also adds the invocation of the property change
            propertyBuilder.SetSetMethod(CreateSetMethod(typeBuilder, getterAndSetterAttributes, propertyName, propertyType, fieldBuilder));

            return propertyBuilder;
        }

        static public FieldBuilder CreatePrivateField(TypeBuilder typeBuilder, Type fieldType, string fieldName)
        {
            return typeBuilder.DefineField("_" + fieldName.ToLowerInvariant(), fieldType, FieldAttributes.Private);
        }

        static public MethodBuilder CreateGetMethod(TypeBuilder typeBuilder, MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            var getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        static public MethodBuilder CreateSetMethod(TypeBuilder typeBuilder, MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
        {
            var methodBuilder = typeBuilder.DefineMethod("set_" + name, attr, null, new[] { type });
            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        static public MethodBuilder CreateGetMethod(TypeBuilder typeBuilder, MethodAttributes attr, 
                                                    string name, Type type,
                                                    IEnumerable<CustomAttributeBuilder> attributes)
        {
            var method = typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            foreach (var attribute in attributes)
                method.SetCustomAttribute(attribute);

            return method;
        }

        private const MethodAttributes ExplicitImplementation =
                    MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final |
                    MethodAttributes.HideBySig | MethodAttributes.NewSlot;

        private const MethodAttributes ImplicitImplementation =
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        [Flags]
        public enum PropertyGetSet { Get = 1, Set = 2, Both = Get | Set }

        static public PropertyBuilder CreatePropertyExplImpl(TypeBuilder typeBuilder, string name,
                                                             Type propertyType, Type interfaceType,
                                                             PropertyGetSet propertyGetSet)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(string.Format("{0}.{1}", interfaceType.FullName, name),
                                                                          PropertyAttributes.HasDefault, propertyType, null);

            if ((propertyGetSet & PropertyGetSet.Get) == PropertyGetSet.Get)
                propertyBuilder.SetGetMethod(CreateGetMethodExplImpl(typeBuilder, name, propertyType, interfaceType));

            if ((propertyGetSet & PropertyGetSet.Set) == PropertyGetSet.Set)
                propertyBuilder.SetSetMethod(CreateSetMethodExplImpl(typeBuilder, name, propertyType, interfaceType));

            return propertyBuilder;
        }

        static public MethodBuilder CreateGetMethodExplImpl(TypeBuilder typeBuilder,
                                                            string name, Type returnType, Type interfaceType)
        {
            MethodInfo interfaceMethod = interfaceType.GetMethod("get_" + name);

            var method = typeBuilder.DefineMethod(string.Format("get_{0}.{1}", interfaceType.FullName, name),
                                                  ExplicitImplementation, returnType, Type.EmptyTypes);

            method.SetReturnType(returnType);

            typeBuilder.DefineMethodOverride(method, interfaceMethod);

            return method;
        }

        static public MethodBuilder CreateSetMethodExplImpl(TypeBuilder typeBuilder,
                                                            string name, Type type, Type interfaceType)
        {
            MethodInfo interfaceMethod = interfaceType.GetMethod("set_" + name);

            var methodBuilder = typeBuilder.DefineMethod(string.Format("set_{0}.{1}", interfaceType.FullName, name),
                                                         ExplicitImplementation, null, new[] { type });

            typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);

            return methodBuilder;
        }
    }
}
