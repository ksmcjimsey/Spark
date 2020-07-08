using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Cars
{
    public class DeleteModel : PageModel
    {
        // Get the EF DB context
        private readonly ApplicationDbContext _db;

        // In the constructor assigned the bound passed
        // in db context to the private local db context variable
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Bind the car object to the view so data can be 
        // passed back and forth
        [BindProperty]
        public Car Car { get; set; }


        // Get will load the data for the initial page
        // ID is passed in by the caller
        public async Task<IActionResult> OnGet(int? id)
        {
            // Check if id is null and return not found
            if (id == null)
            {
                return NotFound();
            }

            // Get the car record to show to the user
            // using Microsoft.EntityFrameworkCore;
            Car = await _db.Car.FirstOrDefaultAsync(c => c.Id == id);

            // Make sure we got something back from db
            if (Car == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Post will delete the vehicle
        // ID will be hidden and passed in
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Car = await _db.Car.FirstOrDefaultAsync(c => c.Id == id);

            // If a car is returned from the DB then we can remove it.
            if (Car != null)
            {
                _db.Car.Remove(Car);
                await _db.SaveChangesAsync();
            }

            return Redirect("/Cars?userId=" + Car.UserId);
        }
    }
}