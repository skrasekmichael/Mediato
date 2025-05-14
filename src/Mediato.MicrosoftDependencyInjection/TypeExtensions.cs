namespace Mediato;

internal static class TypeExtensions
{
	public static Type? GetGenericInterfaceType(this Type type, Type genericInterfaceTypeDef)
	{
		if (!type.IsClass || type.IsAbstract)
		{
			return null;
		}

		return type
			.GetInterfaces()
			.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterfaceTypeDef);
	}
}
