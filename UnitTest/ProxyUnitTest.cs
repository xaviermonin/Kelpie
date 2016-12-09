using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EntityCore;
using System.Data.Entity.Validation;
using EntityCore.DynamicEntity;
using EntityCore.Proxy.Metadata;
using System.Collections.Generic;
using EntityCore.Entity;
using System.Data.Entity;

namespace UnitTest
{
    [TestClass]
    public class ProxyUnitTest
    {
        private DynamicEntityContext entityContext;

        [TestInitialize]
        public void Initialize()
        {
            entityContext = new DynamicEntityContext("name=DataDbContext");
        }

        [TestCleanup]
        public void Cleanup()
        {
            entityContext.Dispose();
            entityContext = null;
        }

        [TestMethod]
        [TestCategory("Proxy")]
        public void RetrieveNavigationPropertyWithLazyLoading()
        {
            Assert.IsTrue(entityContext.Configuration.LazyLoadingEnabled);
            var attribute = entityContext.ProxySet<IAttribute>("Attribute").First();

            Assert.IsNotNull(attribute.Entity);
            Assert.IsInstanceOfType(attribute.Entity, typeof(IEntity));

            Assert.IsNotNull(attribute.Type);
            Assert.IsNotNull(attribute.Type.Attributes);
            Assert.IsNotNull(attribute.Type.Attributes.FirstOrDefault());

            Assert.IsNotNull(attribute.Entity.Proxies);
            Assert.IsNotNull(attribute.Entity.Proxies.FirstOrDefault());
        }

        [TestMethod]
        [TestCategory("Proxy")]
        public void RetrieveSingleProxyEntity()
        {
            IAttributeType stringType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                     .Where(c => c.ClrName == "System.String")
                                                     .Single();

            Assert.AreEqual(stringType.ClrName, "System.String");
            Assert.AreEqual(stringType.SqlServerName, "nvarchar");
        }

        [TestMethod]
        [TestCategory("Proxy")]
        public void RetrieveIncludedProxyEntity()
        {
            var attributesUsingStringType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                            .Include(c => c.Attributes)
                                                            .Where(c => c.ClrName == "System.String")
                                                            .OrderBy(c => c.Id)
                                                            .SelectMany(c => c.Attributes)
                                                            .ToArray();

            Assert.IsInstanceOfType(attributesUsingStringType, typeof(IAttribute[]));
            Assert.AreEqual(attributesUsingStringType.Length, 7);
            Assert.IsNotNull(attributesUsingStringType.FirstOrDefault());
        }

        [TestMethod]
        [TestCategory("Proxy")]
        public void ManyToOneNavigation()
        {
            var attributeName = entityContext.ProxySet<IAttribute>("Attribute")
                                            .Include(c => c.Entity)
                                            .Where(c => c.Name == "Name" &&
                                                        c.Entity.Name == "Entity")
                                            .Single();

            Assert.AreEqual(attributeName.Name, "Name");
            Assert.IsNotNull(attributeName.Entity);
            Assert.AreEqual(attributeName.Entity.Name, "Entity");
            Assert.IsFalse(attributeName.IsNullable.Value);
            Assert.IsTrue(attributeName.Entity.Managed.Value);
        }

        [TestMethod]
        [TestCategory("Proxy")]
        public void OneToManyNavigation()
        {
            var entity = entityContext.ProxySet<IEntity>("Entity")
                                        .Include(c => c.Attributes)
                                        .Where(c => c.Name == "Entity")
                                        .Single();

            Assert.IsNotNull(entity.Attributes);
            Assert.AreEqual(entity.Attributes.Count, 5);

            var nameAttribute = entity.Attributes.Where(c => c.Name == "Name")
                                                    .Single();

            Assert.AreEqual(nameAttribute.Name, "Name");
        }
    }
}
