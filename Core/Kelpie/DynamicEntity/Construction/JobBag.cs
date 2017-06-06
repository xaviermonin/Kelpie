using System.Reflection.Emit;
using Models = Kelpie.Initialization.Metadata.Models;

namespace Kelpie.DynamicEntity.Construction
{
    class JobBag
    {
        public Models.Entity Entity { get; set; }
        public TypeBuilder Type { get; set; }
    }
}
