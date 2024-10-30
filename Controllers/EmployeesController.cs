using LeaveWebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveWebApplication
{

    public class EmployeesController : Controller
    {
        private readonly LeaveManagementDbContext _context;

        public EmployeesController(LeaveManagementDbContext context)
        {
            _context = context;
        }

        // GET: /Employees
        public async Task<IActionResult> Index()
        {
            // Hämtar alla anställda från databasen
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }
    }

}
