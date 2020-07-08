using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Services
{
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // This is new.  We bound to the front end view a list but it
        // does not have a model (object).  It just binds its own 
        // data structure (object) without the need for a model.
        [BindProperty]
        public List<ServiceHeader> ServiceHeader { get; set; }

        // More then one property can be accessed in the view.
        // I think no BindProperty means this is one directional --> to the View
        // We need the user ID to navigate from the service histroy page
        // back to the user page.  We create this property for that reason
        public string UserId { get; set; }


        public HistoryModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // CarId is passed in from the form that calls this page
        // We cheated and did not return to the page or pass Task
        // the IActionResult.  core will auto fill in these two 
        // pieces for us.
        public async Task OnGetAsync(int carId)
        {
            // Include acts as a join and gets the refered to car properties
            // using Microsoft.EntityFrameworkCore;
            // Next pull in the user from the car record using include.
            ServiceHeader = await _db.ServiceHeader.Include(s => s.Car).Include(c => c.Car.ApplicationUser).
                Where(c => c.CarId == carId).ToListAsync();

            // Look up the car based on the carId.  Next create a list (only should be one)
            // and then get the first one and look up the UserId
            UserId = _db.Car.Where(u => u.Id == carId).ToList().FirstOrDefault().UserId;
        }
    }
}