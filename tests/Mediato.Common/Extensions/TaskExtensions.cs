namespace Mediato.Common.Extensions;

public static class TaskExtensions
{
	public static async Task<bool> WithTimeout(this Task task, int milliseconds, CancellationToken ct = default)
	{
		var timeoutTask = Task.Delay(milliseconds, ct);
		var firstTaskToComplete = await Task.WhenAny(task, timeoutTask);

		if (firstTaskToComplete == timeoutTask)
		{
			return true;
		}

		return false;
	}

	public static Task<bool> WithTimeout(this ValueTask task, int milliseconds, CancellationToken ct = default) => task.AsTask().WithTimeout(milliseconds, ct);
}
