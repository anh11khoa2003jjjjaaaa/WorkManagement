using WorkManagement.DTO;
using WorkManagement.Interfaces;
using WorkManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkManagement.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using WorkManagement.Data;

namespace WorkManagement.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly AuthService _authService;
        private readonly EmailService _emailService;
        private readonly WorkManagementContext _dbContext;
        private readonly IRoleService _roleService;
        public AccountController(IRoleService roleService, IAccountService accountService, AuthService authService, EmailService emailService, WorkManagementContext dbContext)
        {
            _accountService = accountService;
            _authService = authService;
            _emailService = emailService;
            _dbContext = dbContext;
            _roleService= roleService;

        }

        // 1. Lấy tất cả tài khoản
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            var skip = (pageNumber - 1) * pageSize;
            var accounts = await _accountService.GetAllAccountsAsync();
            var pagedAccounts = accounts.Skip(skip).Take(pageSize).ToList();

            return Ok(new
            {
                data = pagedAccounts,
                totalCount = accounts.Count(),
                pageNumber = pageNumber,
                pageSize = pageSize
            });
        }

        // 2. Lấy tài khoản theo ID
        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Account>> GetById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        // 3. Tạo tài khoản mới
        [HttpPost("create")]
        public async Task<ActionResult> Create(Account account)
        {
            await _accountService.AddAccountAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        // 4. Cập nhật tài khoản
        [HttpPut("update/{id}")]
        public async Task<ActionResult> Update(int id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            await _accountService.UpdateAccountAsync(account);
            return NoContent();
        }

        // 5. Xóa tài khoản
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }

        // 6. Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var account = await _accountService.GetAccountByUsernameAsync(loginDto.Username);

            if (account == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, account.Password))
            {
                return Unauthorized("Invalid username or password");
            }

            if (account.IsActive != true)
            {
                return Unauthorized("Tài khoản của bạn đã bị vô hiệu hóa.");
            }

            var token = _authService.GenerateJwtToken(account);
            return Ok(new { Token = token });
        }

        // 7. Đăng ký tài khoản
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                // Gọi RegisterAsync từ service để tạo tài khoản
                await _accountService.RegisterAsync(registerDto);

                // Sau khi đăng ký thành công, trả về HTTP 201 (Created)
                return StatusCode(201, "Tài khoản đã được tạo thành công.");
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có bất kỳ vấn đề nào xảy ra
                return BadRequest(ex.Message);
            }
        }

        // 8. Thay đổi mật khẩu
        [HttpPut("changePassword/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePassDto changePassDto)
        {
            if (changePassDto == null)
            {
                return BadRequest("Invalid request body.");
            }

            // Kiểm tra xác nhận mật khẩu
            if (changePassDto.NewPassword != changePassDto.ConfirmNewPassword)
            {
                return BadRequest("Confirmation password does not match.");
            }

            try
            {
                // Thực hiện đổi mật khẩu
                await _accountService.ChangePasswordAsync(id, changePassDto.OldPassword, changePassDto.NewPassword);
                return Ok("Password has been successfully changed.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // Tài khoản không tồn tại
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message); // Mật khẩu hiện tại không đúng
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Lỗi không xác định
            }
        }

        // 9. Quên mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var account = await _accountService.GetAccountByUsernameAsync(request.Username);
            if (account == null)
            {
                return BadRequest("Tài khoản không tồn tại.");
            }

            // Gửi email hoặc OTP để xác minh và tạo link đặt lại mật khẩu
            var otpCode = GenerateOtp();
            var otpToken = new Otp
            {
                AccountId = account.Id,
                Otpcode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiryAt = DateTime.UtcNow.AddMinutes(3),
                IsUsed = false
            };

            _dbContext.Otps.Add(otpToken);
            await _dbContext.SaveChangesAsync();

            var body = $"Mã OTP của bạn là: {otpCode}";
            await _emailService.SendEmailAsync(account.User.Email, "Mã OTP Quên Mật Khẩu", body);

            return Ok("Mã OTP đã được gửi tới email của bạn.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            // Kiểm tra mã OTP và xác minh rằng mã OTP chưa được sử dụng và chưa hết hạn
            var otpToken = await _dbContext.Otps
                .FirstOrDefaultAsync(ot => ot.Otpcode == request.Otpcode && ot.IsUsed == false && ot.ExpiryAt > DateTime.UtcNow);

            if (otpToken == null)
            {
                return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn.");
            }

            // Tìm tài khoản liên kết với OTP
            var account = await _dbContext.Accounts.FindAsync(otpToken.AccountId);
            if (account == null)
            {
                return BadRequest("Tài khoản không tồn tại.");
            }

            // Đặt lại mật khẩu
            account.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Đánh dấu OTP là đã sử dụng
            otpToken.IsUsed = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _dbContext.SaveChangesAsync();

            return Ok("Mật khẩu đã được thay đổi.");
        }


        [HttpPut("resetpassword/{id}")]
        public async Task<IActionResult> ResetPasswordDefaulUsername(int id)
        {
            if(id==null)
            {
                return BadRequest("Tài khoản không tồn tại");
            }
            var account= await _dbContext.Accounts.FindAsync(id);
            if (account == null)
            {
                return BadRequest("Tài khoản không tồn tại.");
            }
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Username);
            await _dbContext.SaveChangesAsync();


            return Ok("Mật khẩu đã được thay đổi trùng với username của bạn!");
        }



        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateAccount(int id, [FromBody] bool isActive)
        {
            if (isActive != true && isActive != false)
            {
                return BadRequest("Trạng thái isActive không hợp lệ.");
            }

            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            account.IsActive = isActive;
            await _accountService.UpdateAccountAsync(account);
            return Ok("Trạng thái tài khoản đã được cập nhật.");
        }


        [HttpPut("assignRole/{id}")]
        public async Task<IActionResult> AssignRole(int id, [FromBody] int roleId)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound("Tài khoản không tồn tại.");
            }

            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest("Vai trò không hợp lệ.");
            }

            account.RoleId = roleId;
            await _accountService.UpdateAccountAsync(account);
            return Ok("Vai trò đã được cập nhật.");
        }
        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
