using System.Collections.Generic;
using System.Data.Entity;

namespace EntityCore.Initialization.Metadata
{
    internal class MetadataInitializer : CreateDatabaseIfNotExists<MetadataContext>
    {
        protected List<Models.AttributeType> attributesTypes = new List<Models.AttributeType>();
        protected List<Models.Entity> entities = new List<Models.Entity>();
        protected List<Models.Proxy> proxies = new List<Models.Proxy>();

        public MetadataInitializer()
        {
            #region AttributeType

            Models.AttributeType stringType = null;
            Models.AttributeType boolType = null;
            Models.AttributeType intType = null;

            attributesTypes.Add(stringType = new Models.AttributeType()
            {
                ClrName = "System.String",
                SqlServerName = "nvarchar"
            });

            attributesTypes.Add(boolType = new Models.AttributeType()
            {
                ClrName = "System.Boolean",
                SqlServerName = "bit"
            });

            attributesTypes.Add(intType = new Models.AttributeType()
            {
                ClrName = "System.Int32",
                SqlServerName = "int"
            });

            attributesTypes.Add(new Models.AttributeType()
            {
                ClrName = "System.Decimal",
                SqlServerName = "real"
            });

            attributesTypes.Add(new Models.AttributeType()
            {
                ClrName = "System.DateTime",
                SqlServerName = "datetime"
            });

            #endregion

            #region Description of Metadata entities

            // Entity

            Models.Entity entityEntity = null;
            Models.Entity attributeTypeEntity = null;
            Models.Entity attributeEntity = null;
            Models.Entity proxyEntity = null;

            entities.Add(entityEntity = new Models.Entity()
            {
                Name = "Entity",
                Description = "Describe an entity",
                Managed = true,
                Metadata = true,
                Attributes =
                {
                    new Models.Attribute()
                    {
                        Name = "Name",
                        IsNullable = false,
                        Type = stringType,
                        Managed = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Description",
                        IsNullable = true,
                        Type = stringType,
                        Managed = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Managed",
                        IsNullable = false,
                        Type = boolType,
                        Managed = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Metadata",
                        IsNullable = false,
                        Type = boolType,
                        Managed = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Association",
                        IsNullable = false,
                        Type = boolType,
                        Managed = true,
                    },
                }
            });

            // Attribute

            entities.Add(attributeEntity = new Models.Entity()
            {
                Name = "Attribute",
                Description = "Describe an entity attribute",
                Managed = true,
                Metadata = true,
                Attributes =
                {
                    new Models.Attribute()
                    {
                        Name = "Name",
                        IsNullable = false,
                        Type = stringType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "IsNullable",
                        IsNullable = true,
                        Type = boolType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "DefaultValue",
                        IsNullable = true,
                        Type = stringType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Length",
                        IsNullable = true,
                        Type = intType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "Managed",
                        IsNullable = false,
                        Type = boolType,
                        Managed = true,
                        Metadata = true,
                    },
                }
            });

            attributeEntity.ManyToOneRelationships.Add(new Models.Relationship()
            {
                Name = "Attributes",
                One = entityEntity,
                OneNavigationName = "Entity",
                Many = attributeEntity,
                ManyNavigationName = "Attributes"
            });

            // AttributeType

            entities.Add(attributeTypeEntity = new Models.Entity()
            {
                Name = "AttributeType",
                Description = "Describe an attribute type",
                Attributes =
                {
                    new Models.Attribute()
                    {
                        Name = "ClrName",
                        IsNullable = true,
                        Type = stringType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "SqlServerName",
                        IsNullable = true,
                        Type = stringType,
                        Managed = true,
                        Metadata = true,
                    },
                    new Models.Attribute()
                    {
                        Name = "DefaultLength",
                        IsNullable = true,
                        Type = intType,
                        Managed = true,
                        Metadata = true,
                    },
                }
            });

            attributeTypeEntity.OneToManyRelationships.Add(new Models.Relationship()
            {
                Name = "Attributes",
                One = attributeTypeEntity,
                OneNavigationName = "Type",
                Many = attributeEntity,
                ManyNavigationName = "Attributes"
            });

            // Proxy

            entities.Add(proxyEntity = new Models.Entity()
                {
                    Name = "Proxy",
                    Description = "Describe a proxy",
                    Attributes =
                    {
                        new Models.Attribute()
                        {
                            Name = "FullyQualifiedTypeName",
                            IsNullable = true,
                            Type = stringType,
                            Managed = true,
                            Metadata = true,
                        },
                        new Models.Attribute()
                        {
                            Name = "Managed",
                            IsNullable = false,
                            Type = boolType,
                            Managed = true,
                            Metadata = true,
                        },
                    }
                });

            proxyEntity.ManyToOneRelationships.Add(new Models.Relationship()
            {
                Name = "Entity",
                One = entityEntity,
                OneNavigationName = "Entity",
                Many = proxyEntity,
                ManyNavigationName = "Proxies"
            });

            #endregion

            #region Proxies

            proxies.Add(new Models.Proxy()
            {
                Entity = attributeTypeEntity,
                Managed = true,
                FullyQualifiedTypeName = typeof(EntityCore.Proxy.Metadata.IAttributeType).AssemblyQualifiedName
            });

            proxies.Add(new Models.Proxy()
            {
                Entity = attributeEntity,
                Managed = true,
                FullyQualifiedTypeName = typeof(EntityCore.Proxy.Metadata.IAttribute).AssemblyQualifiedName
            });

            proxies.Add(new Models.Proxy()
            {
                Entity = entityEntity,
                Managed = true,
                FullyQualifiedTypeName = typeof(EntityCore.Proxy.Metadata.IEntity).AssemblyQualifiedName
            });

            proxies.Add(new Models.Proxy()
            {
                Entity = proxyEntity,
                Managed = true,
                FullyQualifiedTypeName = typeof(EntityCore.Proxy.Metadata.IProxy).AssemblyQualifiedName
            });

            #endregion

            #region Behaviors

            /*businessLogics.Add(new Models.BusinessLogic()
            {
                Entity = entityEntity,
                Managed = true,
                FullyQualifiedTypeName = typeof(EntityCore.DynamicEntity.Behavior.EntityToSqlStructure).AssemblyQualifiedName,
            });*/

            #endregion
        }

        protected override void Seed(MetadataContext context)
        {
            context.AttributeTypes.AddRange(attributesTypes);
            context.Entities.AddRange(entities);
            context.Interfaces.AddRange(proxies);

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
