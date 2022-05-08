namespace CleanArchitecture.Domain.DomainObjects;

public abstract record StronglyTypedIdBase
{
    protected StronglyTypedIdBase(Guid? id)
        => Id = id ?? Guid.NewGuid();

    protected StronglyTypedIdBase(string? id)
        => Id = id is null ? Guid.NewGuid() : new Guid(id);
    
    protected Guid Id { get; set; }
    
    public override string ToString() => Id.ToString();
}