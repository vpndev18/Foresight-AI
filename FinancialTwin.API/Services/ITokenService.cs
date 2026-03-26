using FinancialTwin.API.Models;

namespace FinancialTwin.API.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
