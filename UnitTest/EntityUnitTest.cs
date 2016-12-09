using EntityCore.DynamicEntity;
using EntityCore.Entity;
using EntityCore.Proxy.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class EntityUnitTest
    {
        public interface IUser
        {
            string Firstname { get; set; }
            string Lastname { get; set; }
        }

        public interface IUserLastNameOnly
        {
            string Lastname { get; set; }
        }

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
        public void CreateEntity()
        {
            IAttributeType stringType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                     .Single(attr => attr.ClrName == "System.String");
            IAttributeType intType = entityContext.ProxySet<IAttributeType>("AttributeType")
                                                     .Single(attr => attr.ClrName == "System.Int32");

            var userEntity = entityContext.Create<IEntity>("Entity");
            userEntity.Name = "User";
            userEntity.Description = "Contains some users";

            var firstNameAttribute = entityContext.Create<IAttribute>("Attribute");
            firstNameAttribute.Name = "Firstname";
            firstNameAttribute.Type = stringType;
            firstNameAttribute.IsNullable = true;
            firstNameAttribute.Length = 50;

            var lastNameAttribute = entityContext.Create<IAttribute>("Attribute");
            lastNameAttribute.Name = "Lastname";
            lastNameAttribute.Type = stringType;
            lastNameAttribute.IsNullable = true;
            lastNameAttribute.Length = 50;

            var ageAttribute = entityContext.Create<IAttribute>("Attribute");
            ageAttribute.Name = "Age";
            ageAttribute.Type = intType;
            ageAttribute.IsNullable = true;

            userEntity.Attributes.Add(firstNameAttribute);
            userEntity.Attributes.Add(lastNameAttribute);
            userEntity.Attributes.Add(ageAttribute);

            var userLastnameOnlyProxy = entityContext.Create<IProxy>("Proxy");
            userLastnameOnlyProxy.Entity = userEntity;
            userLastnameOnlyProxy.FullyQualifiedTypeName = typeof(IUserLastNameOnly).AssemblyQualifiedName;

            IProxy userProxy = entityContext.Create<IProxy>("Proxy");
            userProxy.Entity = userEntity;
            userProxy.FullyQualifiedTypeName = typeof(IUser).AssemblyQualifiedName;

            //string query = EntityDatabaseStructure.GenerateCreateTableSqlQuery(userEntity);

            entityContext.ProxySet<IEntity>("Entity").Add(userEntity);
            entityContext.ProxySet<IProxy>("Proxy").Add(userProxy);
            entityContext.ProxySet<IProxy>("Proxy").Add(userLastnameOnlyProxy);
            
            entityContext.SaveChanges();
            //entityContext.Database.ExecuteSqlCommand(query);
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

            var usersSet = context.Set("User") as System.Collections.Generic.IEnumerable<IUser>;
            var xavierFirstName = usersSet.Where(u => u.Firstname == "Xavier").Single();

            var xavierLastName = xavierFirstName as IUserLastNameOnly;

            Assert.AreEqual(xavierFirstName.Firstname, "Xavier");
            Assert.AreEqual(xavierLastName.Lastname, "Monin");

            var usersLastNameSet = context.Set("User") as System.Collections.Generic.IEnumerable<IUserLastNameOnly>;
            xavierLastName = usersLastNameSet.Where(u => u.Lastname == "Hammana").Single();

            Assert.AreEqual(xavierLastName.Lastname, "Hammana");

            dynamic xavierDynamic = xavierFirstName;
            Assert.AreEqual(xavierDynamic.Age, 29);
        }
    }
}
