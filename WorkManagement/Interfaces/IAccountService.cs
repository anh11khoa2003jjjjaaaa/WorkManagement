using WorkManagement.DTO;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetAccountByUsernameAsync(string username);
        Task<Account> GetAccountByIdAsync(int id);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task AddAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(int id);

        Task<Account> LoginAsync(string username, string password);
        Task RegisterAsync(RegisterDto registerDto);
        Task ChangePasswordAsync(int accountId, string currentPassword, string newPassword);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
        Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    }
}
