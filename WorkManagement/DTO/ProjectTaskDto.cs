namespace WorkManagement.DTO
{
    public class ProjectTaskDto
    {

        public int Id { get; set; } // Id của task

        public string Name { get; set; } // Tên task

        public string? Description { get; set; } // Mô tả task (nếu có)

        public int? ProjectId { get; set; } // Id của dự án liên kết (nếu có)

        public int? StatusId { get; set; } 
    }
}
