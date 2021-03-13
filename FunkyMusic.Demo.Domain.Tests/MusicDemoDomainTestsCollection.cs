using FunkyMusic.Demo.Domain.Tests;
using Xunit;

namespace FunkyMusic.Demo.Application.Tests
{
    [CollectionDefinition(Name)]
    public class MusicDemoDomainTestsCollection : ICollectionFixture<TestsInitializer>
    {
        public const string Name = nameof(MusicDemoDomainTestsCollection);
    }
}