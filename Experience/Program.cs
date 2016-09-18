using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Experience
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context("name=DataDbContext");
            ICar voiture = new Car();
            voiture.Name = "208";

            voiture.Roues.Add(new Wheel(){
                Dimension = 10
            });

            context.Voitures.Add(voiture as Car);
            context.SaveChanges();

            var sqlQuery = (context.Voitures as IQueryable<ICar>).Include(c => c.Roues)
                                                                     .Where(c => c.Name == "208")
                                                                     .Select(c => new
                                                                     {
                                                                         Id = c.Id,
                                                                         FirstRoue = c.Roues.FirstOrDefault(),
                                                                         Name = c.Name
                                                                     }).ToString();

            ICar cBon = (context.Voitures as IEnumerable<ICar>).Where(c => c.Name == "208").Single();
        }
    }
}
