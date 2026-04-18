using System.Linq;
using FinancialTwin.API.Services;
using Xunit;

namespace FinancialTwin.Tests
{
    public class FinancialMathEngineTests
    {
        private readonly FinancialMathEngine _engine = new();

        [Fact]
        public void MonteCarlo_ReturnsElevenPointsPerTrajectory_YearZeroPlusTenYears()
        {
            var (p10, p50, p90) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 1000m,
                monthlyContribution: 100m,
                expectedAnnualReturn: 0.07m,
                annualVolatility: 0.15m);

            Assert.Equal(11, p10.Count);
            Assert.Equal(11, p50.Count);
            Assert.Equal(11, p90.Count);
        }

        [Fact]
        public void MonteCarlo_YearZero_EqualsInitialSavings_ForAllPercentiles()
        {
            var (p10, p50, p90) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 5000m,
                monthlyContribution: 200m,
                expectedAnnualReturn: 0.07m,
                annualVolatility: 0.15m);

            Assert.Equal(5000m, p10[0]);
            Assert.Equal(5000m, p50[0]);
            Assert.Equal(5000m, p90[0]);
        }

        [Fact]
        public void MonteCarlo_PercentilesAreOrdered_P10_LE_P50_LE_P90_AtFinalYear()
        {
            var (p10, p50, p90) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 1000m,
                monthlyContribution: 100m,
                expectedAnnualReturn: 0.07m,
                annualVolatility: 0.15m);

            Assert.True(p10.Last() <= p50.Last());
            Assert.True(p50.Last() <= p90.Last());
        }

        [Fact]
        public void MonteCarlo_ZeroVolatility_ZeroReturn_AccumulatesLinearly()
        {
            var (_, p50, _) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 1000m,
                monthlyContribution: 100m,
                expectedAnnualReturn: 0m,
                annualVolatility: 0m);

            // With no return and no volatility, balance grows by 100 * 12 = 1200 per year.
            Assert.Equal(2200m, p50[1]);
            Assert.Equal(13000m, p50[10]);
        }

        [Fact]
        public void MonteCarlo_ZeroVolatility_PositiveReturn_IsDeterministicAndCompounds()
        {
            var (p10, p50, p90) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 10000m,
                monthlyContribution: 0m,
                expectedAnnualReturn: 0.12m,
                annualVolatility: 0m);

            // sigma=0 removes the random term; all paths should be identical and grow.
            Assert.Equal(p10.Last(), p50.Last());
            Assert.Equal(p50.Last(), p90.Last());
            Assert.True(p50.Last() > 10000m);
        }

        [Fact]
        public void MonteCarlo_TrajectoryIsMonotonicNonDecreasing_ForPositiveReturnZeroVolatility()
        {
            var (_, p50, _) = _engine.CalculateMonteCarloTrajectories(
                initialSavings: 1000m,
                monthlyContribution: 50m,
                expectedAnnualReturn: 0.05m,
                annualVolatility: 0m);

            for (int i = 1; i < p50.Count; i++)
            {
                Assert.True(p50[i] >= p50[i - 1],
                    $"Balance decreased from year {i - 1} ({p50[i - 1]}) to year {i} ({p50[i]})");
            }
        }

        [Fact]
        public void MonteCarlo_ExtremeReturn_CapsAtDecimalMaxValue_InsteadOfThrowing()
        {
            // The engine wraps the balance update in try/catch and caps at decimal.MaxValue
            // rather than propagating OverflowException.
            var exception = Record.Exception(() => _engine.CalculateMonteCarloTrajectories(
                initialSavings: 1000m,
                monthlyContribution: 100m,
                expectedAnnualReturn: 800m,
                annualVolatility: 0m));

            Assert.Null(exception);
        }
    }
}
