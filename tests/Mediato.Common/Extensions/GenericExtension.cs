using Shouldly;

namespace Mediato.Common.Extensions;

public static class GenericExtension
{
	public static void ShouldSatisfy<T>(this T obj, Func<T, bool> predicate) => predicate(obj).ShouldBeTrue();
}
