using MediatR;
using FinancialTwin.API.Data;
using FinancialTwin.API.Services;
using Microsoft.EntityFrameworkCore;

namespace FinancialTwin.API.Features.Auth.Queries
{
    public record LoginUserQuery(string Email, string Password) : IRequest<string?>;

    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string?>
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public LoginUserQueryHandler(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return _tokenService.GenerateToken(user);
        }
    }
}
