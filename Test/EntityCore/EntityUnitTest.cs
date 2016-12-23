using EntityCore.DynamicEntity;
using EntityCore.Entity;
using EntityCore.Proxy.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

        private static DynamicEntityContext entityContext;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            entityContext = new DynamicEntityContext(Effort.DbConnectionFactory.CreateTransient(),
                                                     true);
        }

        [ClassCleanup]
        public static void Cleanup()
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

            string query = EntityDatabaseStructure.GenerateCreateTableSqlQuery(userEntity);

            entityContext.ProxySet<IEntity>("Entity").Add(userEntity);
            entityContext.ProxySet<IProxy>("Proxy").Add(userProxy);
            entityContext.ProxySet<IProxy>("Proxy").Add(userLastnameOnlyProxy);
            
            entityContext.SaveChanges();
            entityContext.Database.ExecuteSqlCommand(query);
        }

        [TestMethod]
        public void CreateUsers()
        {
            BaseEntity xm = entityContext.Create("User");

            xm.SetAttributeValue("Firstname", "Han");
            xm.SetAttributeValue("Lastname", "Solo");
            xm.SetAttributeValue("Age", 29);

            entityContext.Set("User").Add(xm);

            dynamic rh = entityContext.Create("User");

            rh.Firstname = "Obi-Wan";
            rh.Lastname = "Kenobi";
            rh.Age = 57;

            entityContext.Set("User").Add(rh);

            entityContext.SaveChanges();
        }

        [TestMethod]
        public void LoadUsers()
        {
            var usersSet = entityContext.Set("User") as System.Collections.Generic.IEnumerable<IUser>;
            var xavierFirstName = usersSet.Where(u => u.Firstname == "Xavier").Single();

            var xavierLastName = xavierFirstName as IUserLastNameOnly;

            Assert.AreEqual(xavierFirstName.Firstname, "Xavier");
            Assert.AreEqual(xavierLastName.Lastname, "Monin");

            var usersLastNameSet = entityContext.Set("User") as System.Collections.Generic.IEnumerable<IUserLastNameOnly>;
            xavierLastName = usersLastNameSet.Where(u => u.Lastname == "Hammana").Single();

            Assert.AreEqual(xavierLastName.Lastname, "Hammana");

            dynamic xavierDynamic = xavierFirstName;
            Assert.AreEqual(xavierDynamic.Age, 29);
        }
    }
}
