using Microsoft.EntityFrameworkCore;
using WorkManagement.Data;
using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BCrypt.Net;
namespace WorkManagement.Service
{
    public class AccountService : IAccountService
    {
        private readonly WorkManagementContext _context;

        public AccountService(WorkManagementContext context)
        {
            _context = context;
        }

        // 1. Thêm tài khoản
        public async Task AddAccountAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        // 2. Cập nhật tài khoản
        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        // 3. Xóa tài khoản
        public async Task DeleteAccountAsync(int id)
        {
            var account = await GetAccountByIdAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        // 4. Tìm kiếm tài khoản theo ID
        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .Include(r => r.Role)
                .Include(c => c.Otps)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // 5. Tìm kiếm tài khoản theo tên người dùng (username)
        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .Include(r => r.Role)
                .FirstOrDefaultAsync(a => a.Username == username);
        }

        // 6. Lấy tất cả tài khoản
        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts.Include(u=>u.User).Include(r=>r.Role).ToListAsync();
        }

        // 7. Đăng nhập (Login)
        public async Task<Account> LoginAsync(string username, string password)
        {
            var account = await _context.Accounts
                .Include(a => a.User)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);

            if (account != null && account.IsActive == true)
            {
                Console.WriteLine($"Role Name: {account.Role?.Name ?? "Role is null"}");
                return account;
            }
            return null;
        }

        // 8. Đăng ký (Register)
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            // Kiểm tra xem tên tài khoản đã tồn tại chưa
            var existingAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == registerDto.Username);

            if (existingAccount != null)
                throw new Exception("Tài khoản đã tồn tại!");

            // Tạo người dùng mới
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                BirthDay = registerDto.BirthDay,
                Phone = registerDto.Phone,
                DepartmentId = null,
                IsLeader=false,
                Avatar = null 
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            // Tạo tài khoản mới
            var account = new Account
            {
                Username = registerDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), // Lưu mật khẩu dưới dạng mã hóa (nếu cần)
                IsActive = true, // Mặc định tài khoản được kích hoạt
                UserId = user.Id, // Liên kết người dùng với tài khoản
                RoleId = 1 // Gán RoleId từ DTO
            };
          
            // Lưu người dùng và tài khoản vào cơ sở dữ liệu
          
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }


        // 9. Thay đổi mật khẩu (Change Password)
        public async Task ChangePasswordAsync(int id, string oldPassword, string newPassword)
        {
            // Tìm tài khoản theo ID
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Tài khoản không tồn tại.");
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, account.Password))
            {
                throw new UnauthorizedAccessException("Mật khẩu hiện tại không chính xác.");
            }

            // Hash mật khẩu mới và cập nhật
            account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Lưu thay đổi
            await _context.SaveChangesAsync();
        }


        // 10. Quên mật khẩu (Forgot Password)
        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == forgotPasswordRequestDto.Username);

            if (account == null)
                throw new Exception("Tài khoản không tồn tại!");

            // Gửi email hoặc OTP để xác minh và tạo link đặt lại mật khẩu
            // Gọi dịch vụ gửi email/OTP ở đây
        }

        // 11. Đặt lại mật khẩu (Reset Password)
        public async Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == resetPasswordRequestDto.Username);

            if (account == null)
                throw new Exception("Tài khoản không tồn tại!");

            account.Password = resetPasswordRequestDto.NewPassword;
            await _context.SaveChangesAsync();
        }
    }
}
