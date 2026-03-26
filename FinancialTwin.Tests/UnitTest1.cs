using Xunit;
using FinancialTwin.API.Services;

namespace FinancialTwin.Tests
{
    public class FinancialMathEngineTests
    {
        [Fact]
        public void Calculate10YearTrajectory_ShouldAddMonthlyContributionsCorrectly_WithZeroInterest()
        {
            // Arrange (Set up the test data)
            var engine = new FinancialMathEngine();
            decimal initialSavings = 1000m;
            decimal monthlyContribution = 100m;
            decimal annualReturnRate = 0m; // 0% return so we just test addition

            // Act (Run the method we want to test)
            var trajectory = engine.Calculate10YearTrajectory(initialSavings, monthlyContribution, annualReturnRate);

            // Assert (Verify the results are mathematically correct)
            // Trajectory should be exactly 11 items long (Year 0 baseline + 10 years)
            Assert.Equal(11, trajectory.Count);

            // Year 0 should be the initial savings
            Assert.Equal(1000m, trajectory[0]);

            // Year 1 should be $1000 + ($100 * 12 months) = $2200
            Assert.Equal(2200m, trajectory[1]);

            // Year 10 should be $1000 + ($100 * 120 months) = $13000
            Assert.Equal(13000m, trajectory[10]);
        }
        
        [Fact]
        public void Calculate10YearTrajectory_ShouldCompoundInterestCorrectly()
        {
            // Arrange
            var engine = new FinancialMathEngine();
            decimal initialSavings = 10000m; // $10k
            decimal monthlyContribution = 0m; // No extra contributions
            decimal annualReturnRate = 0.12m; // 12% annual return (1% per month)

            // Act
            var trajectory = engine.Calculate10YearTrajectory(initialSavings, monthlyContribution, annualReturnRate);

            // Assert
            // 1% compounded monthly for 12 months on $10k
            // Math: 10000 * (1.01)^12 = ~11268.25
            var expectedYear1 = Math.Round(10000m * (decimal)Math.Pow(1.01, 12), 2);
            
            // Allow a small margin of floating point rounding error in the assert
            Assert.Equal(expectedYear1, trajectory[1], 2); 
        }
        
        [Fact]
        public void Calculate10YearTrajectory_ShouldCatchOverflowExplosions()
        {
            // Arrange
            var engine = new FinancialMathEngine();
            decimal initialSavings = 1000m; 
            decimal monthlyContribution = 100m; 
            decimal annualReturnRate = 800m; // 80,000% return! This will trigger an OverflowException.

            // Act
            var trajectory = engine.Calculate10YearTrajectory(initialSavings, monthlyContribution, annualReturnRate);

            // Assert
            // Our try-catch block should catch it and cap it at decimal.MaxValue
            Assert.Equal(decimal.MaxValue, trajectory[10]); 
        }
    }
}
