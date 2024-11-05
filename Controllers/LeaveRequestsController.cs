using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LeaveWebApplication.Data;
using LeaveWebApplication.Models;

namespace LeaveWebApplication.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private readonly LeaveManagementDbContext _context;

        public LeaveRequestsController(LeaveManagementDbContext context)
        {
            _context = context;
        }

        // GET: LeaveRequests
        public async Task<IActionResult> Index()
        {
            var leaveManagementDbContext = _context.LeaveRequests.Include(l => l.Employee).Include(l => l.LeaveType);
            return View(await leaveManagementDbContext.ToListAsync());
        }

        // GET: LeaveRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // GET: LeaveRequests/Create
        public IActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "Name");
            ViewBag.TypeId = new SelectList(_context.LeaveTypes, "TypeId", "LeaveName");
            return View();
        }

        // POST: LeaveRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,EndDate,TypeId,EmployeeId")] LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    leaveRequest.ApplicationDate = DateTime.Now; // Set the application date to the current date
                    _context.Add(leaveRequest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                    Console.WriteLine($"An error occurred while saving the leave request: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            // Reset the dropdown lists if the model state is not valid
            ViewBag.EmployeeId = new SelectList(_context.Employees, "EmployeeId", "Name", leaveRequest.EmployeeId);
            ViewBag.TypeId = new SelectList(_context.LeaveTypes, "TypeId", "LeaveName", leaveRequest.TypeId);
            return View(leaveRequest);
        }


        // GET: LeaveRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", leaveRequest.EmployeeId);
            ViewData["TypeId"] = new SelectList(_context.LeaveTypes, "TypeId", "TypeId", leaveRequest.TypeId);
            return View(leaveRequest);
        }

        // POST: LeaveRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,StartDate,EndDate,TypeId,ApplicationDate,EmployeeId")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.RequestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", leaveRequest.EmployeeId);
            ViewData["TypeId"] = new SelectList(_context.LeaveTypes, "TypeId", "TypeId", leaveRequest.TypeId);
            return View(leaveRequest);
        }

        // GET: LeaveRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // POST: LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveRequests/Search
        public async Task<IActionResult> Search(string employeeName, int? month, int? year)
        {
            var leaveRequests = _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .AsQueryable();

            // Filtrate based on employee name
            if (!string.IsNullOrEmpty(employeeName))
            {
                leaveRequests = leaveRequests.Where(l => l.Employee.Name.Contains(employeeName));
            }

            // Filtrate based on month and year
            if (month.HasValue && year.HasValue)
            {
                var startDate = new DateTime(year.Value, month.Value, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                leaveRequests = leaveRequests.Where(l => l.StartDate <= endDate && l.EndDate >= startDate);
            }

            var results = await leaveRequests.ToListAsync();

            // Return the search results to the view
            return View(results);
        }
        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.RequestId == id);
        }


    }
}
