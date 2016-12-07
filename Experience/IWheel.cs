using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Experience
{
    public interface IWheel
    {
        int Id { get; set; }

        int Size { get; set; }

        ICar Proprietaire { get; set; }

        ICar GetProprietaire();
        void SetProprietaire(ICar car);
    }
}
