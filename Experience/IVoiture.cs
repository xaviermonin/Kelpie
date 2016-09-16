using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Experience
{
    public interface IVoiture
    {
        int Id { get; set; }

        string Name { get; set; }

        ICollection<IRoue> Roues { get; }
    }
}
