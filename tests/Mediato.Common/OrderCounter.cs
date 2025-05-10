namespace Mediato.Common;

public sealed class OrderCounter
{
	private readonly Lock _mutex = new();

	public int Count { get; private set; } = 0;

	public int Invoke()
	{
		lock (_mutex)
		{
			Count++;
		}

		return Count;
	}

	public void Reset()
	{
		lock (_mutex)
		{
			Count = 0;
		}
	}
}
