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
    public interface IUserFirstName
    {
        string Firstname { get; set; }
        string Lastname { get; set; }
    }

    public interface IUserLastName
    {
        string Lastname { get; set; }
    }

    [TestClass]
    public class EntityUnitTest
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

        [TestMethod]
        public void CreateEntity()
        {
            /*ICollection<EntityCore.Proxy.IBaseEntity> a = new HashSet<AttributeTest>();
            ICollection<EntityCore.Proxy.Metadata.IAttribute> b = a as ICollection<EntityCore.Proxy.Metadata.IAttribute>;*/

            /*
            IEntity userEntity = entityContext.Create<IEntity>("Entity");
            userEntity.Name = "User";
            userEntity.Description = "Contains all users";
            userEntity.Attributes.Add(new EntityCore.Models.Attribute()
            {
                Name = "Firstname",
                Type = stringType,
                IsNullable = true,
                Length = 50
            });
            userEntity.Attributes.Add(new EntityCore.Models.Attribute()
            {
                Name = "Lastname",
                Type = stringType,
                IsNullable = true,
                Length = 50
            });
            userEntity.Attributes.Add(new EntityCore.Models.Attribute()
            {
                Name = "Age",
                Type = intType,
                IsNullable = true
            });

            Interface userInterfaceLastname = new Interface()
            {
                Entity = userEntity,
                FullyQualifiedTypeName = typeof(IUserLastName).AssemblyQualifiedName
            };

            Interface userInterfaceFirstname = new Interface()
            {
                Entity = userEntity,
                FullyQualifiedTypeName = typeof(IUserFirstName).AssemblyQualifiedName
            };

            string query = EntityCore.DynamicEntity.EntityDatabaseStructure.GenerateCreateTableSqlQuery(userEntity);

            entityContext.MetadataContext.Entities.Add(userEntity);
            entityContext.MetadataContext.Interfaces.Add(userInterfaceLastname);
            entityContext.MetadataContext.Interfaces.Add(userInterfaceFirstname);
            
            entityContext.MetadataContext.SaveChanges();
            entityContext.MetadataContext.Database.ExecuteSqlCommand(query);*/
        }

        [TestMethod]
        public void CreateUsers()
        {
            DynamicEntityContext context = new DynamicEntityContext("name=DataDbContext");
            //EntityContext context = new EntityContext("name=DataDbContext");

            BaseEntity xm = context.Create("User");

            xm.SetAttributeValue("Firstname", "Xavier");
            xm.SetAttributeValue("Lastname", "Monin");
            xm.SetAttributeValue("Age", 29);

            context.Set("User").Add(xm);

            dynamic rh = context.Create("User");

            rh.Firstname = "Charles-Henri";
            rh.Lastname = "Hammana";
            rh.Age = 29;

            context.Set("User").Add(rh);

            context.SaveChanges();
        }

        [TestMethod]
        public void LoadUsers()
        {
            DynamicEntityContext context = new DynamicEntityContext("name=DataDbContext");

            var usersSet = context.Set("User") as System.Collections.Generic.IEnumerable<IUserFirstName>;
            var xavierFirstName = usersSet.Where(u => u.Firstname == "Xavier").Single();

            var xavierLastName = xavierFirstName as IUserLastName;

            Assert.AreEqual(xavierFirstName.Firstname, "Xavier");
            Assert.AreEqual(xavierLastName.Lastname, "Monin");

            var usersLastNameSet = context.Set("User") as System.Collections.Generic.IEnumerable<IUserLastName>;
            xavierLastName = usersLastNameSet.Where(u => u.Lastname == "Hammana").Single();

            Assert.AreEqual(xavierLastName.Lastname, "Hammana");

            dynamic xavierDynamic = xavierFirstName;
            Assert.AreEqual(xavierDynamic.Age, 29);
        }
    }
}
