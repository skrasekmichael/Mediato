using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Mediato.MicrosoftDependencyInjection.Tests.RegistrationTests.Extensions;

public static class ServiceDescriptorExtensions
{
	public static bool Matches(this ServiceDescriptor descriptorA, ServiceDescriptor descriptorB)
	{
		return descriptorA.Lifetime == descriptorB.Lifetime
			&& descriptorA.ServiceType == descriptorB.ServiceType
			&& descriptorA.ImplementationType == descriptorB.ImplementationType;
	}

	public static void ShouldMatch(this ServiceDescriptor descriptorA, ServiceDescriptor descriptorB)
	{
		descriptorA.Lifetime.ShouldBe(descriptorB.Lifetime);
		descriptorA.ServiceType.ShouldBe(descriptorB.ServiceType);
		descriptorA.ImplementationType.ShouldBe(descriptorB.ImplementationType);
	}
}
