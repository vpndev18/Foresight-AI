using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialTwin.API.Services
{
    public interface IFinancialMathEngine
    {
        // 10,000 iteration Monte Carlo Simulation using random variable sampling
        (List<decimal> P10, List<decimal> P50, List<decimal> P90) CalculateMonteCarloTrajectories(
            decimal initialSavings, 
            decimal monthlyContribution, 
            decimal expectedAnnualReturn, 
            decimal annualVolatility);
    }

    public class FinancialMathEngine : IFinancialMathEngine
    {
        public (List<decimal> P10, List<decimal> P50, List<decimal> P90) CalculateMonteCarloTrajectories(
            decimal initialSavings, 
            decimal monthlyContribution, 
            decimal expectedAnnualReturn, 
            decimal annualVolatility)
        {
            int iterations = 1000;
            int years = 10;
            
            // Standardizing inputs for monthly calculations
            decimal monthlyExpectedReturn = expectedAnnualReturn / 12m;
            decimal monthlyVolatility = annualVolatility / (decimal)Math.Sqrt(12);
            
            var allPaths = new List<List<decimal>>();
            var random = new Random();

            for (int i = 0; i < iterations; i++)
            {
                var path = new List<decimal> { Math.Round(initialSavings, 2) };
                decimal currentBalance = initialSavings;

                for (int year = 1; year <= years; year++)
                {
                    for (int month = 1; month <= 12; month++)
                    {
                        // 1. Box-Muller transform for Standard Normal Distribution (Bell Curve) random sampling
                        double u1 = 1.0 - random.NextDouble(); 
                        double u2 = 1.0 - random.NextDouble();
                        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                        // 2. Adjust standard normal by our specified Market Volatility and Mean Return
                        decimal randomMonthlyReturn = monthlyExpectedReturn + monthlyVolatility * (decimal)randStdNormal;

                        try
                        {
                            // 3. Apply the randomized market return to our portfolio
                            currentBalance += currentBalance * randomMonthlyReturn;
                            // 4. Add the monthly contribution
                            currentBalance += monthlyContribution;
                        }
                        catch (OverflowException)
                        {
                            currentBalance = decimal.MaxValue;
                            break;
                        }
                    }
                    
                    // We record the balance securely at the end of each year to limit JSON payload sizes
                    path.Add(Math.Round(currentBalance, 2));
                }
                
                allPaths.Add(path);
            }

            // 5. Sort all 1,000 parallel universes by their final 10-year ending balance
            var sortedPaths = allPaths.OrderBy(p => p.Last()).ToList();

            // 6. Extract our 3 percentile lines
            int p10Index = (int)(iterations * 0.10); // 10th percentile (Worst Case Scenario)
            int p50Index = (int)(iterations * 0.50); // 50th percentile (Median Case Scenario)
            int p90Index = (int)(iterations * 0.90); // 90th percentile (Best Case Scenario)

            return (sortedPaths[p10Index], sortedPaths[p50Index], sortedPaths[p90Index]);
        }
    }
}
