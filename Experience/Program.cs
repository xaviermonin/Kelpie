using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Experience
{
    class Program
    {
        static private void TestProxies()
        {
            Context context = new Context("name=DataDbContext");
            ICar voiture = new Car();
            voiture.Name = "208";

            /*voiture.Roues.Add(new Wheel()
            {
                Dimension = 10
            });

            context.Voitures.Add(voiture as Car);
            context.SaveChanges();*/

            var sqlQuery = (context.Voitures as IQueryable<ICar>).Include(c => c.Roues)
                                                                     .Where(c => c.Name == "208")
                                                                     .Select(c => new
                                                                     {
                                                                         Id = c.Id,
                                                                         FirstRoue = c.Roues.FirstOrDefault(),
                                                                         Name = c.Name
                                                                     }).ToString();

            ICar cBon = (context.Voitures as IEnumerable<ICar>).Where(c => c.Name == "208").Single();
            var results = (context.Voitures as IQueryable<ICar>).Include(c => c.Roues).First().Roues.ToArray();
        }

        static private void TestTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "tmpAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("tmpModule");

            TypeBuilder typeBuilderOfA = module.DefineType("A", TypeAttributes.Public | TypeAttributes.Class);
            TypeBuilder typeBuilderOfB = module.DefineType("B", TypeAttributes.Public | TypeAttributes.Class);

            typeBuilderOfA.DefineField("B", typeBuilderOfB, FieldAttributes.Public);
            typeBuilderOfB.DefineField("A", typeBuilderOfA, FieldAttributes.Public);

            Type aType = typeBuilderOfA.CreateType();
            Type bType = typeBuilderOfB.CreateType();

            dynamic a = Activator.CreateInstance(aType);
            dynamic b = Activator.CreateInstance(bType);

            a.B = b;
            b.A = a;
        }

        static void Main(string[] args)
        {
            TestProxies();
            TestTypeBuilder();
        }
    }
}
