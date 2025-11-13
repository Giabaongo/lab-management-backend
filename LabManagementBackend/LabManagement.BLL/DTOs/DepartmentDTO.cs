namespace LabManagement.BLL.DTOs
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsUserMember { get; set; }
        public bool CanRegister { get; set; } = true;
    }
}
