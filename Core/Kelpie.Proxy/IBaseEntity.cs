
namespace Kelpie.Proxy
{
    public interface IBaseEntity
    {
        int Id { get; set; }

        T GetAttributeValue<T>(string propertyName);
        void SetAttributeValue<T>(string propertyName, T value);

        void AddMemberToRelationship(string relationName, IBaseEntity baseEntity);
    }
}
