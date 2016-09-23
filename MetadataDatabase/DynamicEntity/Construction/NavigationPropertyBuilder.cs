using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EntityCore.Initialization.Metadata.Models;
using Models = EntityCore.Initialization.Metadata.Models;
using System.Reflection;

namespace EntityCore.DynamicEntity.Construction
{
    class NavigationPropertyBuilder
    {
        private System.Reflection.Emit.TypeBuilder _typeBuilder;

        public NavigationPropertyBuilder(System.Reflection.Emit.TypeBuilder _typeBuilder)
        {
            this._typeBuilder = _typeBuilder;
        }

        internal void CreateNavigationProperties(IEnumerable<Models.Relationship> oneToManyRelationship,
                                                 IEnumerable<Models.Relationship> manyToOneRelationship,
                                                 IEnumerable<Type> availablesTypes = null)
        {
            foreach  (var manyToOne in manyToOneRelationship)
                CreateFieldForType(manyToOne, availablesTypes);
        }

        private void CreateFieldForType(Relationship manyToOneRelationship, IEnumerable<Type> availablesTypes)
        {
            Type fieldType = null;

            fieldType = availablesTypes.Where(c => c.Name == manyToOneRelationship.One.Name).Single();

            FieldBuilder fieldBuilder = _typeBuilder.DefineField("_" + manyToOneRelationship.OneNavigationName.ToLowerInvariant(), fieldType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = _typeBuilder.DefineProperty(manyToOneRelationship.OneNavigationName, PropertyAttributes.HasDefault, fieldType, null);

            MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual;

            //creates the Get Method for the property
            propertyBuilder.SetGetMethod(CreateGetMethod(getterAndSetterAttributes, manyToOneRelationship.OneNavigationName, fieldType, fieldBuilder));
            //creates the Set Method for the property and also adds the invocation of the property change
            propertyBuilder.SetSetMethod(CreateSetMethod(getterAndSetterAttributes, manyToOneRelationship.OneNavigationName, fieldType, fieldBuilder));
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
            var methodBuilder = _typeBuilder.DefineMethod("set" + name, attr, null, new[] { type });
            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, fieldBuilder);
            generator.Emit(OpCodes.Ret);

            return methodBuilder;
        }
    }
}
