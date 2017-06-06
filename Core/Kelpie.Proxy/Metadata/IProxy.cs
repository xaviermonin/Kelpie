namespace Kelpie.Proxy.Metadata
{
    [BindedEntity(Name = "Proxy")]
    public interface IProxy : IBaseEntity
    {
        [BindedNavigationProperty]
        IEntity Entity { get; set; }

        string FullyQualifiedTypeName { get; set; }

        bool Managed { get; set; }
    }
}
