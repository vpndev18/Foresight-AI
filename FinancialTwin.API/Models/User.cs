namespace FinancialTwin.API.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal CurrentSavings { get; set; } = 0;
        public string Currency { get; set; } = "USD"; // e.g., "USD", "EUR", "INR"
        
        public string PasswordHash { get; set; } = string.Empty;

        // A user can run multiple branching simulations over time:
        public List<Simulation> Simulations { get; set; } = new();
    }
}
