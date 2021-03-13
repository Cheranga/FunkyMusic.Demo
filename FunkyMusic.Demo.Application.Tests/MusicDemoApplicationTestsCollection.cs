using Xunit;

namespace FunkyMusic.Demo.Application.Tests
{
    [CollectionDefinition(Name)]
    public class MusicDemoApplicationTestsCollection : ICollectionFixture<TestsInitializer>
    {
        public const string Name = nameof(MusicDemoApplicationTestsCollection);
    }
}