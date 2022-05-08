using CleanArchitecture.Domain.DomainObjects;

namespace CleanArchitecture.Laboratory;
public partial class EntityLabClass : IEntity
{
    public string? Name { get; set; }

    public EntityLabClass()
    {
        Name = Id.ToString();
    }
}
