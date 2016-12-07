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
            ICar car = new Car();
            car.Name = "208";

            car.Wheels.Add(new Wheel()
            {
                Size = 10
            });

            context.Cars.Add(car as Car);
            context.SaveChanges();

            var sqlQuery = (context.Cars as IQueryable<ICar>).Include(c => c.Wheels)
                                                                     .Where(c => c.Name == "208")
                                                                     .Select(c => new
                                                                     {
                                                                         Id = c.Id,
                                                                         FirstRoue = c.Wheels.FirstOrDefault(),
                                                                         Name = c.Name
                                                                     }).ToString();

            ICar cBon = (context.Cars as IEnumerable<ICar>).Where(c => c.Name == "208").Single();
            var results = (context.Cars as IQueryable<ICar>).Include(c => c.Wheels).First().Wheels;

            foreach (var r in results.ToArrayTest())
            {
                var a = r.GetProprietaire(); // Force build of this method in release
                r.SetProprietaire(a);

                Console.Write(r);
            }
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
