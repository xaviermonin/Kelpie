using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Voiture : IVoiture
    {
        public Voiture()
        {
            this.Roues = new HashSet<Roue>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Roue> Roues { get; private set; }

        private BindingCollection<Roue, IRoue> bindingCollectionRoues;

        [NotMapped]
        ICollection<IRoue> IVoiture.Roues
        {
            get {
                if (bindingCollectionRoues == null)
                    bindingCollectionRoues = new BindingCollection<Roue, IRoue>(this.Roues);

                return bindingCollectionRoues;
            }
        }
    }
}
