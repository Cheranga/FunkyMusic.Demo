using Xunit;

namespace FunkyMusic.Demo.Api.Tests
{
    [CollectionDefinition(Name)]
    public class MusicDemoApiTestsCollection : ICollectionFixture<TestsInitializer>
    {
        public const string Name = nameof(MusicDemoApiTestsCollection);
    }
}