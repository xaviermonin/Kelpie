using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    interface IEntityWorkshop
    {
        void DoWork(IEnumerable<JobBag> entities);
    }
}
