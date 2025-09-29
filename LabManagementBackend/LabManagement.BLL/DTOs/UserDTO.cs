namespace LabManagement.BLL.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public decimal Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}