using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Experience
{
    public interface IRoue
    {
        int Id { get; set; }

        int Dimension { get; set; }

        IVoiture Proprietaire { get; set; }
    }
}
