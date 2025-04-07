namespace WorkManagement.DTO
{
    public class UserDto
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public DateTime? BirthDay { get; set; }

        public IFormFile? Avatar { get; set; }

        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public bool? isLeader { get; set; }

    }
}
