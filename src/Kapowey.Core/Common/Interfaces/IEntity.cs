namespace Kapowey.Core.Common.Interfaces;

public interface IEntity
{

}
public interface IEntity<T> : IEntity
{
    T Id { get; set; }
}