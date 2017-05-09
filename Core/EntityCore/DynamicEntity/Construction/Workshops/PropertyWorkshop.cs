using EntityCore.DynamicEntity.Construction.Helper.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction.Workshops
{
    class PropertyWorkshop : Workshop<PropertyWorkshop.Result>
    {
        public class Result : WorkshopResult
        {
            public IEnumerable<PropertyBuilder> Properties { get; set; }
        }

        public PropertyWorkshop(EntityFactory factory) : base(factory)
        {
        }

        protected override Result DoWork(Models.Entity entity, TypeBuilder typeBuilder)
        {
            return new Result()
            {
                Properties = CreateProperties(entity.Attributes, typeBuilder)
            };
        }

        public IEnumerable<PropertyBuilder> CreateProperties(ICollection<Models.Attribute> properties, TypeBuilder typeBuilder)
        {
            return properties.ToList().Select(p => CreateFieldForType(p, typeBuilder)).ToArray();
        }

        private PropertyBuilder CreateFieldForType(Models.Attribute attribute, TypeBuilder typeBuilder)
        {
            Type fieldType = null;

            if (attribute.IsNullable)
                fieldType = TypeHelper.GetNullableType(Type.GetType(attribute.Type.ClrName));
            else
                fieldType = Type.GetType(attribute.Type.ClrName);

            var propertyBuilder = PropertyHelper.CreateAutoProperty(typeBuilder, attribute.Name,
                                                                    fieldType, PropertyHelper.PropertyGetSet.Both);

            //add the various WCF and EF attributes to the property
            AddDataMemberAttribute(propertyBuilder);

            // @todo : Add SQL attribute only for SQL context.
            // This don't work with Effort
            AddColumnAttribute(propertyBuilder, attribute.Type.SqlServerName);

            return propertyBuilder;
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
    }
}
