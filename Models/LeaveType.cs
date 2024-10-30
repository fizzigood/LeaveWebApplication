using System.ComponentModel.DataAnnotations;

namespace LeaveWebApplication.Models
{
    public class LeaveType
    {
        [Key]
        public int TypeId { get; set; }
        public string LeaveName { get; set; }

        // Navigation property
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
