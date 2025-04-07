namespace WorkManagement.DTO
{
    public class TaskEmployeeDto
    {
        //public int? TaskId { get; set; }  // ID công việc mà nhân viên được giao
        //public int? UserId { get; set; }  // ID nhân viên được giao công việc
        //public DateTime? AssignedAt { get; set; }// Thời gian giao việc
        //public int? StatusId { get; set; }
        //public DateTime Deadline { get; set; }

        //public DateTime? CompletionDate { get; set; }

        //public bool? PenaltyStatus { get; set; }
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? StatusId { get; set; }
        public string? Description { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool? PenaltyStatus { get; set; }
    }
}
