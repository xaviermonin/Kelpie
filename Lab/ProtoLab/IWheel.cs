using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtoLab
{
    public interface IWheel
    {
        int Id { get; set; }

        int Size { get; set; }

        ICar Owner { get; set; }

        ICar GetProprietaire();
        void SetProprietaire(ICar car);
    }
}
