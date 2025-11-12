namespace LabManagement.Common.Constants
{
    public static class Constant
    {
        public const int MaxDepartmentsPerMember = 2;

        public enum UserRole
        {
            Admin = 0,
            SchoolManager = 1,
            LabManager = 2,
            SecurityLab = 3,
            Member = 4
        }
    }
}
