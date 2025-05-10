namespace Mediato.Common;

public sealed class Owner<TOwner, TService>(TService service)
{
	public TService Service { get; } = service;
}

public sealed class Owner<TOwner, TSubOwner, TService>(TService service)
{
	public TService Service { get; } = service;
}
