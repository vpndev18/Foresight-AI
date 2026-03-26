using MediatR;
using FinancialTwin.API.Data;
using FinancialTwin.API.Models;
using FinancialTwin.API.Services;

namespace FinancialTwin.API.Features.Simulations.Commands
{
    // The Command Payload
    public record RunSimulationCommand(Guid UserId, string Title, decimal MonthlyContribution, decimal ExpectedAnnualReturn, decimal AnnualVolatility) : IRequest<Simulation>;

    // The Handler
    public class RunSimulationCommandHandler : IRequestHandler<RunSimulationCommand, Simulation>
    {
        private readonly AppDbContext _context;
        private readonly IFinancialMathEngine _mathEngine;
        private readonly IAiService _aiService;

        public RunSimulationCommandHandler(AppDbContext context, IFinancialMathEngine mathEngine, IAiService aiService)
        {
            _context = context;
            _mathEngine = mathEngine;
            _aiService = aiService;
        }

        public async Task<Simulation> Handle(RunSimulationCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the user from the database
            var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
            if (user == null)
                throw new Exception("User not found!");

            // 2. Run the math simulation (Monte Carlo)
            var (p10, p50, p90) = _mathEngine.CalculateMonteCarloTrajectories(
                user.CurrentSavings, 
                request.MonthlyContribution, 
                request.ExpectedAnnualReturn,
                request.AnnualVolatility);
            
            // 3. Format median trajectory and percentiles into a string for the AI
            string formattedTrajectoryP50 = string.Join(" -> ", p50.Select(t => $"{user.Currency} {t:N0}"));
            
            // 4. Build the prompt for the AI service
            var prompt = $"Based on savings of {user.Currency} {user.CurrentSavings:N0}, contributing {user.Currency} {request.MonthlyContribution}/mo at {request.ExpectedAnnualReturn * 100}% expected return with {request.AnnualVolatility * 100}% volatility. The median 10-year path is: {formattedTrajectoryP50}. The worst-case 10th percentile ending balance is {user.Currency} {p10.Last():N0} and the best-case 90th percentile is {user.Currency} {p90.Last():N0}. Give short financial advice regarding this risk spread.";

            // 5. Get AI-generated financial advice
            var simulation = new Simulation
            {
                Title = request.Title,
                UserId = request.UserId,
                InitialSavings = user.CurrentSavings,
                MonthlyContribution = request.MonthlyContribution,
                AnnualReturnRate = request.ExpectedAnnualReturn,
                AnnualVolatility = request.AnnualVolatility,
                P10Trajectory = p10,
                P50Trajectory = p50,
                P90Trajectory = p90,
                AiGeneratedStory = await _aiService.GetAdviceAsync(prompt)
            };
            // 5. Save to the Database
            _context.Simulations.Add(simulation);
            await _context.SaveChangesAsync(cancellationToken);

            return simulation;
        }
    }
}
