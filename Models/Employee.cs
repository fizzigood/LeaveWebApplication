using System.ComponentModel.DataAnnotations;

namespace LeaveWebApplication.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // Navigation property
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
