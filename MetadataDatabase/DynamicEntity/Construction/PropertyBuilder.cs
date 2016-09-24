using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    class PropertyFactory
    {
        //private readonly MethodInfo setPropertyMethod;
        private System.Reflection.Emit.TypeBuilder _typeBuilder;

        public PropertyFactory(System.Reflection.Emit.TypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
            //setPropertyMethod = typeof(BaseEntity).GetMethod("SetProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void CreateProperties(ICollection<Models.Attribute> properties)
        {
            properties.ToList().ForEach(p => CreateFieldForType(p));
        }

        private void CreateFieldForType(Models.Attribute attribute)
        {
            Type fieldType = null;

            if (attribute.IsNullable)
                fieldType = TypeHelper.GetNullableType(Type.GetType(attribute.Type.ClrName));
            else
                fieldType = Type.GetType(attribute.Type.ClrName);

            var propertyBuilder = TypeHelper.CreateProperty(_typeBuilder, attribute.Name, fieldType);

            //add the various WCF and EF attributes to the property
            AddDataMemberAttribute(propertyBuilder);
            AddColumnAttribute(propertyBuilder, attribute.Type.SqlServerName);
        }

        /*public void AddDataServiceKeyAttribute()
        {
            Type attrType = typeof(DataServiceKeyAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { "Id" }));
        }*/

        private void AddDataMemberAttribute(PropertyBuilder propertyBuilder)
        {
            Type attrType = typeof(DataMemberAttribute);
            var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { });
            propertyBuilder.SetCustomAttribute(attr);
        }

        private void AddColumnAttribute(PropertyBuilder propertyBuilder, string sqlServerTypeName)
        {
            Type attrType = typeof(ColumnAttribute);

            PropertyInfo typeName = attrType.GetProperty("TypeName");

            var attr = new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes), new object[] { },
                                                                           new PropertyInfo[] { typeName },
                                                                           new object[] { sqlServerTypeName });
            propertyBuilder.SetCustomAttribute(attr);
        }

        /*private MethodBuilder CreateGetMethod(MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
        {
            var getMethodBuilder = _typeBuilder.DefineMethod("get_" + name, attr, type, Type.EmptyTypes);

            var getMethodILGenerator = getMethodBuilder.GetILGenerator();
            getMethodILGenerator.Emit(OpCodes.Ldarg_0);
            getMethodILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodILGenerator.Emit(OpCodes.Ret);

            return getMethodBuilder;
        }

        private MethodBuilder CreateSetMethod(MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
        {
            var setMethodBuilder = _typeBuilder.DefineMethod("set_" + name, attr, null, new Type[] { type });

            var setMethodILGenerator = setMethodBuilder.GetILGenerator();

            var setPropertyMethodInstance = setPropertyMethod.MakeGenericMethod(type);

            // SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)

            setMethodILGenerator.Emit(OpCodes.Nop);
            setMethodILGenerator.Emit(OpCodes.Ldarg_0); // this
            setMethodILGenerator.Emit(OpCodes.Ldarg_0); // this
            setMethodILGenerator.Emit(OpCodes.Ldflda, fieldBuilder); // Reference of field
            setMethodILGenerator.Emit(OpCodes.Ldarg_1);
            setMethodILGenerator.Emit(OpCodes.Ldstr, name); // name
            setMethodILGenerator.EmitCall(OpCodes.Call, setPropertyMethodInstance, null);
            setMethodILGenerator.Emit(OpCodes.Pop);
            setMethodILGenerator.Emit(OpCodes.Ret);

            return setMethodBuilder;
        }*/
    }
}
