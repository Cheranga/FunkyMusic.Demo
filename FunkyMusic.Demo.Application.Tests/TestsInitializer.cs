using AutoFixture;

namespace FunkyMusic.Demo.Application.Tests
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