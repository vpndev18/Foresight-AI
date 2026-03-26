using MediatR;
using FinancialTwin.API.Data;
using FinancialTwin.API.Models;

namespace FinancialTwin.API.Features.Users.Queries
{
    // The Query: carries the authenticated user's ID from the JWT
    public record GetCurrentUserQuery(Guid UserId) : IRequest<User?>;

    // The Handler: fetches the user from the database
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, User?>
    {
        private readonly AppDbContext _context;

        public GetCurrentUserQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        }
    }
}
