using Xunit;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests
{
    [CollectionDefinition(Name)]
    public class MusicDemoInfrastructureApiTestsCollection : ICollectionFixture<TestsInitializer>
    {
        public const string Name = nameof(MusicDemoInfrastructureApiTestsCollection);
    }
}