using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EntityCore.DynamicEntity.Construction.Helper.Reflection
{
    static class FieldHelper
    {
        /// <summary>
        /// Create a private field
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="fieldType"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        static public FieldBuilder CreatePrivateField(TypeBuilder typeBuilder, Type fieldType, string fieldName)
        {
            return typeBuilder.DefineField("_" + fieldName.ToLowerInvariant(), fieldType, FieldAttributes.Private);
        }
    }
}
