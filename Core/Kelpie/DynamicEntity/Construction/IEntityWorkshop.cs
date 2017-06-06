using System.Collections.Generic;

namespace Kelpie.DynamicEntity.Construction
{
    interface IEntityWorkshop
    {
        void DoWork(IEnumerable<JobBag> entities);
    }
}
