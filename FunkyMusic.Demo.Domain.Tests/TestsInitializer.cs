using AutoFixture;

namespace FunkyMusic.Demo.Domain.Tests
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