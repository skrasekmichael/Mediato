using System.Diagnostics;
using Shouldly;

namespace Mediato.Common;

public record RuntimeStatistics(long Start, long Finish)
{
	public static RuntimeStatistics Get(long start)
	{
		var finish = Stopwatch.GetTimestamp();
		return new RuntimeStatistics(start, finish);
	}

	public override string ToString() => $"{new TimeSpan(Start)} - {new TimeSpan(Finish)} ({new TimeSpan(Finish - Start)})";

	public void ShouldBeInRange(RuntimeStatistics runtime)
	{
		ShouldBeInRange(runtime.Start, runtime.Finish);
	}

	public void ShouldBeInRange(long start, long finish)
	{
		new TimeSpan(start).ShouldBeLessThanOrEqualTo(new TimeSpan(Start));
		new TimeSpan(finish).ShouldBeGreaterThanOrEqualTo(new TimeSpan(Finish));
	}
}
