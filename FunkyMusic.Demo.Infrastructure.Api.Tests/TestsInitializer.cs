using AutoFixture;

namespace FunkyMusic.Demo.Infrastructure.Api.Tests
{
    public class TestsInitializer
    {
        public TestsInitializer()
        {
            Fixture = new Fixture();
        }

        public Fixture Fixture { get; set; }
    }
}