using MediatR;
using Microsoft.EntityFrameworkCore;
using FinancialTwin.API.Data;
using FinancialTwin.API.Models;

namespace FinancialTwin.API.Features.Simulations.Queries
{
    // The Query: carries the authenticated user's ID to filter simulations
    public record GetUserSimulationsQuery(Guid UserId) : IRequest<List<Simulation>>;

    // The Handler: fetches all simulations belonging to this user
    public class GetUserSimulationsQueryHandler : IRequestHandler<GetUserSimulationsQuery, List<Simulation>>
    {
        private readonly AppDbContext _context;

        public GetUserSimulationsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Simulation>> Handle(GetUserSimulationsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Simulations
                .Where(s => s.UserId == request.UserId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
