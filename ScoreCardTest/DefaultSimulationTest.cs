using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ScoreCardTest
{
    public class DefaultSimulationTest
    {
        private readonly ITestOutputHelper _output;

        public DefaultSimulationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private Tuple<Simulator, int> Run()
        {
            var sim = new Simulator();
            const int turns = 10000;
            var finishedIn = sim.Run(turns);
            sim.IsSolutionKnownByMe.Should().BeTrue("{0} should have been enough to figure it out.", turns);
            var nonSolutionCards = sim.PlayerHands.Values.SelectMany(x => x).Except(sim.Solution).ToArray();
            foreach (var card in nonSolutionCards)
            {
                card.IsPartOfAccusation.Should().BeFalse();
            }
            return new Tuple<Simulator, int>(sim, finishedIn);
        }

        [Fact]
        private void RunOnce()
        {
            var results = Run();
            _output.WriteLine($"Solved '{string.Join(",", results.Item1.Solution.Select(c => c.Name))}' in {results.Item2} turns.");
        }

        [Fact]
        private void RunManyTimes()
        {
            var turns = Enumerable.Repeat(0, 2000).Select(x => Run().Item2).ToArray();
            _output.WriteLine($"Average: {turns.Average()}");
            _output.WriteLine($"Min: {turns.Min()}");
            _output.WriteLine($"Max: {turns.Max()}");
        }
    }
}
