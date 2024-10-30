using System.ComponentModel.DataAnnotations;

namespace LeaveWebApplication.Models
{
    public class LeaveRequest
    {
        [Key]
        public int RequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TypeId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int EmployeeId { get; set; }


        // Navigation properties
        public Employee Employee { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
