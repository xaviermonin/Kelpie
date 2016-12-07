using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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

            var getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        /// <summary>
        /// Create a setter method for property that affected the field property
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

        /// <summary>
        /// Create a getter method for property with explicit implementation
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        static public MethodBuilder CreateGetMethodExplImpl(TypeBuilder typeBuilder, string name,
                                                            Type returnType, Type interfaceType)
        {
            MethodInfo interfaceMethod = interfaceType.GetMethod("get_" + name);

            var method = typeBuilder.DefineMethod(string.Format("get_{0}.{1}", interfaceType.FullName, name),
                                                  ExplicitImplementation, returnType, Type.EmptyTypes);

            method.SetReturnType(returnType);

            typeBuilder.DefineMethodOverride(method, interfaceMethod);

            return method;
        }

        /// <summary>
        /// Create a setter method for property with explicit implementation
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        static public MethodBuilder CreateSetMethodExplImpl(TypeBuilder typeBuilder, string name,
                                                            Type type, Type interfaceType)
        {
            MethodInfo interfaceMethod = interfaceType.GetMethod("set_" + name);

            var methodBuilder = typeBuilder.DefineMethod(string.Format("set_{0}.{1}", interfaceType.FullName, name),
                                                         ExplicitImplementation, null, new[] { type });

            typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);

            return methodBuilder;
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
