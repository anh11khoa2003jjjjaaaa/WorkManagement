namespace WorkManagement.DTO
{
    public class ResetPasswordRequestDto
    {
        public string Username { get; set; }
        public string Otpcode { get; set; } = null!;

        public string NewPassword { get; set; }
    }
}
