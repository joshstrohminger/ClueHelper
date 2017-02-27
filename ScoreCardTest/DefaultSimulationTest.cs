using FluentAssertions;
using Xunit;

namespace ScoreCardTest
{
    public class DefaultSimulationTest
    {
        [Fact]
        public void RunOnce()
        {
            var sim = new Simulator();
            const int turns = 1000;
            sim.Run(turns);
            sim.IsSolutionKnownByMe.Should().BeTrue("{0} should have been enough to figure it out.", turns);
        }
    }
}
