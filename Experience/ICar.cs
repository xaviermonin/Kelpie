﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Experience
{
    public interface ICar
    {
        int Id { get; set; }

        string Name { get; set; }

        ICollection<IWheel> Roues { get; }

        ICollection<IWheel> Test();
    }
}
