namespace WorkManagement.DTO
{
    public class ProjectDto
    {
        //public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusId { get; set; }
        public int ? DepartmentId { get; set; }
        public int? ManagerId { get; set; }
    }
}
