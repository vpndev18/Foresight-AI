using MediatR;
using FinancialTwin.API.Data;
using FinancialTwin.API.Models;

namespace FinancialTwin.API.Features.Users.Commands
{
    // 1. The Command: This is the payload (what the client sends us)
    public record CreateUserCommand(string Name, string Email, decimal CurrentSavings, string Currency = "USD") : IRequest<Guid>;

    // 2. The Handler: The logic that executes when a CreateUserCommand is received
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly AppDbContext _context;

        public CreateUserCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Map the command payload to a database entity
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                CurrentSavings = request.CurrentSavings,
                Currency = request.Currency
            };

            // Save to Neon Postgres!
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Return the new Guid so the frontend can route to their profile
            return user.Id;
        }
    }
}
