namespace EntityCore.Proxy.Metadata
{
    [BindedEntity(Name = "Listener")]
    public interface IListener : IBaseEntity
    {
        [BindedNavigationProperty]
        IEntity Entity { get; set; }

        string FullyQualifiedTypeName { get; set; }

        bool Managed { get; set; }
    }
}
