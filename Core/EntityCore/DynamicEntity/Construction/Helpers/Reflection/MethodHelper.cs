using EntityCore.DynamicEntity.Construction.Helpers.Emitter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EntityCore.DynamicEntity.Construction.Helper.Reflection
{
    static class MethodHelper
    {
        /// <summary>
        /// Create a getter method that return the field property
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="attr"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="fieldBuilder"></param>
        /// <returns></returns>
        static public MethodBuilder CreateGetMethod(TypeBuilder typeBuilder, MethodAttributes attr,
                                                    string name, Type type, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            var generator = new EmitHelper(getMethodBuilder.GetILGenerator());

            generator.ldarg_0()
                     .ldfld(fieldBuilder)
                     .ret();

            return getMethodBuilder;
        }

        /// <summary>
        /// Create a setter method for property that assign the field property
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="attr"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="fieldBuilder"></param>
        /// <returns></returns>
        static public MethodBuilder CreateSetMethod(TypeBuilder typeBuilder, MethodAttributes attr,
                                                    string name, Type type, FieldBuilder fieldBuilder)
        {
            var methodBuilder = typeBuilder.DefineMethod("set_" + name, attr, null, new[] { type });
            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        /// <summary>
        /// Create a empty getter method for property
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="attr"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        static public MethodBuilder CreateEmptyGetMethod(TypeBuilder typeBuilder, MethodAttributes attr,
                                                    string name, Type type,
                                                    IEnumerable<CustomAttributeBuilder> attributes)
        {
            var method = typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            foreach (var attribute in attributes)
                method.SetCustomAttribute(attribute);

            return method;
        }

        static public MethodBuilder CreateExplImplMethod(TypeBuilder typeBuilder, string name,
                                                         Type returnType, Type interfaceType,
                                                         Type[] parameterTypes)
        {
            MethodInfo interfaceMethod = interfaceType.GetMethod(name);

            var method = typeBuilder.DefineMethod(string.Format("{0}.{1}", interfaceType.FullName, name),
                                                  ExplicitImplementation, returnType, parameterTypes);

            typeBuilder.DefineMethodOverride(method, interfaceMethod);

            return method;
        }

        /// <summary>
        /// MethodsAttributes of an explicit implementation
        /// </summary>
        private const MethodAttributes ExplicitImplementation =
                    MethodAttributes.Private | MethodAttributes.Virtual | MethodAttributes.Final |
                    MethodAttributes.HideBySig | MethodAttributes.NewSlot;

        /// <summary>
        /// MethodsAttributes of an implicit implemntation
        /// </summary>
        private const MethodAttributes ImplicitImplementation =
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;
    }
}
