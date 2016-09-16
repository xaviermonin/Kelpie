using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Roue : IRoue
    {
        [Key]
        public int Id { get; set; }

        public int Dimension { get; set; }

        public Voiture Proprietaire { get; set; }

        [NotMapped]
        IVoiture IRoue.Proprietaire
        {
            get
            {
                return Proprietaire;
            }
            set
            {
                Proprietaire = value as Voiture;
            }
        }
    }
}
