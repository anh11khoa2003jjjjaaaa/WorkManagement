namespace WorkManagement.DTO
{
    public class CreateTaskEmployeeDto
    {
        public int? TaskId { get; set; }
        public int? UserId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime Deadline { get; set; }
        
    }
}
