using EntityCore.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity
{
    internal class EntityFactory
    {
        private AppDomain _appDomain;
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private TypeBuilder _typeBuilder;
        private string _assemblyName;
        private readonly MethodInfo setPropertyMethod;

        public EntityFactory()
            : this("Entity.Objects")
        {

        }

        public EntityFactory(string assemblyName)
        {
            _appDomain = Thread.GetDomain();
            _assemblyName = assemblyName;

            setPropertyMethod = typeof(BaseEntity).GetMethod("SetProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// This is the normal entry point and just return the Type generated at runtime
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Type CreateDynamicType<T>(Models.Entity entity) where T : BaseEntity
        {
            var tb = CreateDynamicTypeBuilder<T>(entity);
            return tb.CreateType();
        }

        /// <summary>
        /// Exposes a TypeBuilder that can be returned and created outside of the class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public TypeBuilder CreateDynamicTypeBuilder<T>(Models.Entity entity)
            where T : BaseEntity
        {
            if (_assemblyBuilder == null)
                _assemblyBuilder = _appDomain.DefineDynamicAssembly(new AssemblyName(_assemblyName),
                    AssemblyBuilderAccess.RunAndSave);

            //vital to ensure the namespace of the assembly is the same as the module name, else IL inspectors will fail
            if (_moduleBuilder == null)
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyName + ".dll");

            Type[] interfaces = entity.Interfaces.Select(i => Type.GetType(i.FullyQualifiedTypeName)).ToArray();

            //typeof(T) is for the base class, can be omitted if not needed
            _typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + entity.Name, TypeAttributes.Public
                                                            | TypeAttributes.Class
                                                            | TypeAttributes.AutoClass
                                                            | TypeAttributes.AnsiClass
                                                            | TypeAttributes.Serializable
                                                            | TypeAttributes.BeforeFieldInit, typeof(T), interfaces);

            //various class based attributes for WCF and EF
            AddDataContractAttribute();
            AddTableAttribute(entity.Name);
            //AddDataServiceKeyAttribute();

            CreateProperties(_typeBuilder, entity.Attributes);

            return _typeBuilder;
        }

        public void AddDataContractAttribute()
        {
            Type attrType = typeof(DataContractAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(Type.EmptyTypes),
                new object[] { }));
        }

        public void AddTableAttribute(string name)
        {
            Type attrType = typeof(TableAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { name }));
        }

        /*public void AddDataServiceKeyAttribute()
        {
            Type attrType = typeof(DataServiceKeyAttribute);
            _typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(attrType.GetConstructor(new[] { typeof(string) }),
                new object[] { "Id" }));
        }*/

        public void CreateProperties(TypeBuilder typeBuilder, ICollection<Models.Attribute> properties)
        {
            properties.ToList().ForEach(p => CreateFieldForType(p));
        }

        private void CreateFieldForType(Models.Attribute attribute)
        {
            Type fieldType = Type.GetType(attribute.Type.ClrName);

            FieldBuilder fieldBuilder = _typeBuilder.DefineField("_" + attribute.Name.ToLowerInvariant(), fieldType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(attribute.Name, PropertyAttributes.HasDefault, fieldType, null);

            //add the various WCF and EF attributes to the property
            AddDataMemberAttribute(propertyBuilder);
            AddColumnAttribute(propertyBuilder, attribute.Type.SqlServerName);

            MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            //creates the Get Method for the property
            propertyBuilder.SetGetMethod(CreateGetMethod(getterAndSetterAttributes, attribute.Name, fieldType, fieldBuilder));
            //creates the Set Method for the property and also adds the invocation of the property change
            propertyBuilder.SetSetMethod(CreateSetMethod(getterAndSetterAttributes, attribute.Name, fieldType, fieldBuilder));
        }

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

        private MethodBuilder CreateGetMethod(MethodAttributes attr, string name, Type type, FieldBuilder fieldBuilder)
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
        }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save(_assemblyBuilder.GetName().Name + ".dll");
        }
    }
}
