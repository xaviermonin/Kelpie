using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Wheel : IWheel
    {
        [Key]
        public int Id { get; set; }

        public int Dimension { get; set; }

        public Car Proprietaire { get; set; }

        [NotMapped]
        ICar IWheel.Proprietaire
        {
            get
            {
                return Proprietaire;
            }
            set
            {
                Proprietaire = value as Car;
            }
        }

        ICar IWheel.GetProprietaire()
        {
            return Proprietaire;
        }

        void IWheel.SetProprietaire(ICar car)
        {
            Proprietaire = car as Car;
        }
    }
}
