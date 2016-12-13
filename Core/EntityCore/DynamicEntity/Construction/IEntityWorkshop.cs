using System.Collections.Generic;

namespace EntityCore.DynamicEntity.Construction
{
    interface IEntityWorkshop
    {
        void DoWork(IEnumerable<JobBag> entities);
    }
}
