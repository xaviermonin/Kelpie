using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experience
{
    class Car : ICar
    {
        public Car()
        {
            this.Roues = new HashSet<Wheel>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Wheel> Roues { get; private set; }

        private BindingCollection<Wheel, IWheel> bindingCollectionRoues;

        ICollection<IWheel> ICar.Test()
        {
                if (bindingCollectionRoues == null)
                    bindingCollectionRoues = new BindingCollection<Wheel, IWheel>(this.Roues);

                return bindingCollectionRoues;
        }

        [NotMapped]
        ICollection<IWheel> ICar.Roues
        {
            get {
                if (bindingCollectionRoues == null)
                    bindingCollectionRoues = new BindingCollection<Wheel, IWheel>(this.Roues);

                return bindingCollectionRoues;
            }
        }
    }
}
