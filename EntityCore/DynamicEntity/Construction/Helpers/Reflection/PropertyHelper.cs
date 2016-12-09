using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EntityCore.DynamicEntity.Construction.Helper.Reflection
{
    static class PropertyHelper
    {
        /// <summary>
        /// Qualify getter and/or setter of a property
        /// </summary>
        [Flags]
        public enum PropertyGetSet
        {
            Get = 0x01,
            Set = 0x02,
            Both = Get | Set
        }
        
        /// <summary>
        /// Create a property (get and set) with backing field.
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        static public PropertyBuilder CreateFullProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = FieldHelper.CreatePrivateField(typeBuilder, propertyType, propertyName);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodAttributes getterAndSetterAttributes = MethodAttributes.Public | MethodAttributes.SpecialName |
                                                         MethodAttributes.HideBySig | MethodAttributes.Virtual;

            // Creates the Get Method for the property
            propertyBuilder.SetGetMethod(MethodHelper.CreateGetMethod(typeBuilder, getterAndSetterAttributes, propertyName, propertyType, fieldBuilder));
            // Creates the Set Method for the property and also adds the invocation of the property change
            propertyBuilder.SetSetMethod(MethodHelper.CreateSetMethod(typeBuilder, getterAndSetterAttributes, propertyName, propertyType, fieldBuilder));

            return propertyBuilder;
        }

        /// <summary>
        /// Create the Set or/and Get property with an explicit implementation of an interface.
        /// </summary>
        /// <param name="typeBuilder">Type </param>
        /// <param name="name">Property name</param>
        /// <param name="propertyType">Property type</param>
        /// <param name="interfaceType">Interface type</param>
        /// <param name="propertyGetSet">Get or/and Set property</param>
        /// <returns></returns>
        static public PropertyBuilder CreatePropertyExplImpl(TypeBuilder typeBuilder, string name,
                                                             Type propertyType, Type interfaceType,
                                                             PropertyGetSet propertyGetSet)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(string.Format("{0}.{1}", interfaceType.FullName, name),
                                                                         PropertyAttributes.HasDefault, propertyType, null);

            if ((propertyGetSet & PropertyGetSet.Get) == PropertyGetSet.Get)
            {
                var getMethod = MethodHelper.CreateExplImplMethod(typeBuilder, "get_" + name,
                                                                  propertyType, interfaceType, Type.EmptyTypes);
                propertyBuilder.SetGetMethod(getMethod);
            }
            
            if ((propertyGetSet & PropertyGetSet.Set) == PropertyGetSet.Set)
            {
                var setMethod = MethodHelper.CreateExplImplMethod(typeBuilder, "set_" + name, null,
                                                                  interfaceType, new Type[] { propertyType });
                propertyBuilder.SetSetMethod(setMethod);
            }

            return propertyBuilder;
        }
    }
}
