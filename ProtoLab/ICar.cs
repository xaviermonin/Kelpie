using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProtoLab
{
    public interface ICar
    {
        int Id { get; set; }

        string Name { get; set; }

        ICollection<IWheel> Wheels { get; }

        ICollection<IWheel> Test();
    }
}
