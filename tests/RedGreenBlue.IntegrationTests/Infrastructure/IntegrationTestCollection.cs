using Xunit;

namespace RedGreenBlue.IntegrationTests.Infrastructure;

[CollectionDefinition("Integration", DisableParallelization = true)]
public class IntegrationTestCollection : ICollectionFixture<TestWebApplicationFactory>
{
}
