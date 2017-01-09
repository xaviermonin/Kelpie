using EntityCore;
using EntityCore.DynamicEntity;
using EntityCore.Entity;
using EntityCore.Proxy;
using EntityCore.Proxy.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class EntityUnitTest
    {
        private DynamicEntityContext entityContext;

        [TestInitialize]
        public void Initialize()
        {
            entityContext = Context.New(Effort.DbConnectionFactory.CreatePersistent("EntityUnitTest"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            entityContext.Dispose();
            entityContext = null;
        }
        
        [TestMethod]
        [TestCategory("Entity")]
        public void CheckDatabaseCleanup()
        {
            try
            {
                entityContext.Set("EnsureDatabaseCleanup");
                Assert.Fail("'EnsureDatabaseCleanup' must be not loaded");
            }
            catch (ArgumentException) { }

            Assert.IsFalse(entityContext.ProxySet<IEntity>().Where(e => e.Name == "EnsureDatabaseCleanup").Any());

            var entity = entityContext.ProxySet<IEntity>().Create();
            entity.Name = "EnsureDatabaseCleanup";
            entityContext.ProxySet<IEntity>().Add(entity);
            entityContext.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void CreateWithOneToManyRelationship()
        {
            IBaseEntity stringType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                  .Single(attr => attr.ClrName == "System.String");

            IBaseEntity entity = entityContext.Create("Entity");
            entity.SetAttributeValue("Name", "CreateWithManyToOneRelationship");

            IBaseEntity attribute = entityContext.Create("Attribute");
            attribute.SetAttributeValue("Name", "Attribute_CreateWithOneToManyRelationship");

            throw new NotImplementedException("AddMemberToRelationship(RelationName, Member);");
            //entity.AddMemberToRelationship("Attributes", attribute);

            entityContext.Set("Entity").Add(entity);

            entityContext.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void CreateWithManyToOneRelationship()
        {
            IBaseEntity stringType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                  .Single(attr => attr.ClrName == "System.String");

            IBaseEntity entity = entityContext.Create("Entity");
            entity.SetAttributeValue("Name", "CreateWithManyToOneRelationship");

            IBaseEntity attribute = entityContext.Create("Attribute");
            attribute.SetAttributeValue("Name", "Attribute_CreateWithManyToOneRelationship");
            attribute.SetAttributeValue("Entity", entity);

            entityContext.Set("Attribute").Add(attribute);

            entityContext.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void CreateEntityWithBaseEntity()
        {
            IBaseEntity entity = entityContext.Create("Entity");
            entity.SetAttributeValue("Name", "CreateEntityWithBaseEntity");
            entityContext.Set("Entity").Add(entity);

            entityContext.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void CreateEntityWithDynamic()
        {
            dynamic entity = entityContext.Create("Entity");
            entity.Name = "CreateEntityWithDynamic";
            entityContext.Set("Entity").Add(entity);

            entityContext.SaveChanges();
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void RetrieveWithDynamic()
        {
            var entities = entityContext.Set("Entity") as IEnumerable<dynamic>;
            var entityEntity = entities.Where(e => e.Name == "Entity").Single();

            Assert.AreEqual(entityEntity.Name, "Entity");
            Assert.AreEqual(entityEntity.Managed, true);
        }

        [TestMethod]
        [TestCategory("Entity")]
        public void RetrieveWithBaseEntity()
        {
            var entities = entityContext.Set("Entity") as IEnumerable<dynamic>;
            BaseEntity entityEntity = entities.Where(e => e.Name == "Entity").Single();

            Assert.AreEqual(entityEntity.GetAttributeValue<string>("Name"), "Entity");
            Assert.AreEqual(entityEntity.GetAttributeValue<bool>("Managed"), true);
        }
    }
}
