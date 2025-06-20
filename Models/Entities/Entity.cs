namespace Domain.Abstractions;

public abstract class Entity
{
    public required Guid Id { get; init; }
}