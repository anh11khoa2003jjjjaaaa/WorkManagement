namespace WorkManagement.DTO
{
    public class UpdateTaskEmployeeDto
    {
        public int? TaskId { get; set; }

        public int? UserId { get; set; }

        public DateTime? AssignedAt { get; set; }

        public int? StatusId { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime? CompletionDate { get; set; }

        public bool? PenaltyStatus { get; set; }
    }
}
