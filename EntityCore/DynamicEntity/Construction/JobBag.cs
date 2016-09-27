using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = EntityCore.Initialization.Metadata.Models;
using System.Reflection.Emit;

namespace EntityCore.DynamicEntity.Construction
{
    class JobBag
    {
        public Models.Entity Entity { get; set; }
        public TypeBuilder Type { get; set; }
    }
}
