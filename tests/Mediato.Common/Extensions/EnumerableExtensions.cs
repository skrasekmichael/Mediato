using System;
using Shouldly;

namespace Mediato.Common.Extensions;

public static class EnumerableExtensions
{
	public static void ShouldAllSatisfy<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
	{
		foreach (var item in enumerable)
		{
			item.ShouldSatisfy(predicate);
		}
	}

	public static void ShouldAllSatisfy<T>(this IEnumerable<T> enumerable, Func<T, int, bool> predicate)
	{
		int index = 0;
		foreach (var item in enumerable)
		{
			predicate(item, index).ShouldBe(true);
			index++;
		}
	}
}
