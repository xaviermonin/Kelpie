using System.Reflection.Emit;
using Models = EntityCore.Initialization.Metadata.Models;

namespace EntityCore.DynamicEntity.Construction
{
    class JobBag
    {
        public Models.Entity Entity { get; set; }
        public TypeBuilder Type { get; set; }
    }
}
