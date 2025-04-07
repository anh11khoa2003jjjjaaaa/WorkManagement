using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(Account account);
    }
}
