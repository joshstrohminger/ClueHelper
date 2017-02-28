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

        private static Tuple<Simulator, int> Run()
        {
            var sim = new Simulator();
            const int turns = 20000;
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
            var turns = Enumerable.Repeat(0, 3000).Select(x => Run().Item2).ToArray();
            var average = turns.Average();
            var allCounts = turns.GroupBy(x => x).Select(x => new {Turns = x.Key, Count = x.Count()}).OrderByDescending(x => x.Count).ToArray();
            var most = allCounts[0].Count;
            var modes = allCounts.Where(x => x.Count == most).Select(x => x.Turns).OrderBy(x => x).ToList();
            var ordered = turns.OrderBy(x => x).ToArray();
            var offset = (ordered.Length - 1) / 2;
            var median = (ordered[offset] + ordered[ordered.Length - offset]) / 2.0;
            var sumOfSquaresOfDifferences = turns.Select(val => (val - average) * (val - average)).Sum();
            var sd = Math.Sqrt(sumOfSquaresOfDifferences / turns.Length);
            _output.WriteLine($"Runs:    {turns.Length}");
            _output.WriteLine($"Average: {average:N}");
            _output.WriteLine($"StdDev:  {sd:N}");
            _output.WriteLine($"Mode:    {string.Join(",", modes)} occurred {most} times");
            _output.WriteLine($"Median:  {median:N}");
            _output.WriteLine($"Min:     {turns.Min()}");
            _output.WriteLine($"Max:     {turns.Max()}");
        }
    }
}
