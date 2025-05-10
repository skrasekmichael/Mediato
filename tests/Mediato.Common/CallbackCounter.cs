
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mediato.Common.Extensions;

namespace Mediato.Common;

public sealed class CallbackCounter
{
	private readonly Guid _identifier = Guid.CreateVersion7();
	private readonly SemaphoreSlim _mutex = new(1);

	public int Count { get; private set; } = 0;

	private TaskCompletionSource<int> _tcs = new();

	public async Task InvokeAsync([CallerFilePath] string? source = null)
	{
		DebugLog(source, "invoking");
		await _mutex.WaitAsync();

		Count++;
		_tcs.SetResult(Count);
		_tcs = new();

		_mutex.Release();
		DebugLog(source, "invoked");
	}

	public void Reset()
	{
		lock (_mutex)
		{
			Count = 0;
			_tcs = new();
		}
	}

	public async Task WaitForCallbackAsync() => await _tcs.Task;

	public Task WaitForFirstCallbackAsync([CallerFilePath] string? source = null) => WaitForNthCallbackAsync(1, source);

	public Task WaitForSecondCallbackAsync([CallerFilePath] string? source = null) => WaitForNthCallbackAsync(2, source);

	public async Task WaitForNthCallbackAsync(int n, [CallerFilePath] string? source = null)
	{
		DebugLog(source, $"waiting for {n}. invoke of");

		await _mutex.WaitAsync();
		int current = Count;
		_mutex.Release();

		if (current >= n)
		{
			DebugLog(source, $"{n}. waiting completed instantaneously");
			return;
		}

		do
		{
			await _mutex.WaitAsync(); //enforce resetting tcs before awaiting it
			DebugLog(source, $"still waiting for {n}. invoke of");
			_mutex.Release();

			current = await _tcs.Task;
		} while (current < n);

		DebugLog(source, $"{n}. waiting completed");
	}

	public override string ToString() => $"{_identifier}: {Count} ({_tcs.Task.Status})";

	[Conditional("DEBUG")]
	private void DebugLog(string? source, string message)
	{
		var caller = Path.GetFileNameWithoutExtension(source);
		Debug.WriteLine($"{caller} {message,-40} {this}");
	}
}
