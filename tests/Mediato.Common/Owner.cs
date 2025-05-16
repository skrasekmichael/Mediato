namespace Mediato.Common;

public interface IOwner<TService>
{
	public TService Service { get; }
}

public sealed class Owner<TOwner, TService>(TService service) : IOwner<TService>
{
	public TService Service { get; } = service;
}

public sealed class Owner<TOwner, TSubOwner, TService>(TService service) : IOwner<TService>
{
	public TService Service { get; } = service;
}
