using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Program
    {
        static void Main(string[] args)
        {
            Context context = new Context("name=DataDbContext");
            IVoiture voiture = new Voiture();
            voiture.Name = "208";

            voiture.Roues.Add(new Roue(){
                Dimension = 10
            });

            context.Voitures.Add(voiture as Voiture);
            context.SaveChanges();

            IVoiture cBon = context.Voitures.Where(c => c.Name == "208").Single();
        }
    }
}
