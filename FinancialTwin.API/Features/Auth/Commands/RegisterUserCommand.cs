using MediatR;
using FinancialTwin.API.Data;
using FinancialTwin.API.Models;
using FinancialTwin.API.Services;
using BCrypt.Net;

namespace FinancialTwin.API.Features.Auth.Commands
{
    public record RegisterUserCommand(string Name, string Email, string Password, decimal CurrentSavings, string Currency = "USD") : IRequest<string>;

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public RegisterUserCommandHandler(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Simple check if email exists
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                throw new Exception("User with this email already exists.");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                CurrentSavings = request.CurrentSavings,
                Currency = request.Currency,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Return a JWT token immediately upon registration
            return _tokenService.GenerateToken(user);
        }
    }
}
