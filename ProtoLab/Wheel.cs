using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoLab
{
    class Wheel : IWheel
    {
        [Key]
        public int Id { get; set; }

        public int Size { get; set; }

        public Car Proprietaire { get; set; }

        [NotMapped]
        ICar IWheel.Owner
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
