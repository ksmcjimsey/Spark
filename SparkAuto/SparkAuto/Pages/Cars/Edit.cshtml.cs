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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // db passed in by 
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Car Car { get; set; }


        // Called when edit button is clicked to show the user the 
        // the editable car details.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check if id is null and return not found
            if (id == null)
            {
                return NotFound();
            }

            // Get the car record to show to the user
            Car = await _db.Car.FirstOrDefaultAsync(c => c.Id == id);

            // Make sure we got something back from db
            if (Car == null)
            {
                return NotFound();
            }

            return Page();
        }
        

        // Used once the user has edited the car and wants to submit the changes
        public async Task<IActionResult> OnPostAsync()
        {
            // Check that the edit page submitted a valid model
            if (!ModelState.IsValid)
            {
                return Page();  // Not valid return to the page
            }

            else
            {
                // Car is a bound value so the returned object is from the users page
                var carInDb = await _db.Car.SingleOrDefaultAsync
                    (c => c.Id == Car.Id);

                // Verify we have a valid object back from the DB
                if(carInDb == null)
                {
                    return NotFound();
                }

                // Save the values for the object to the db
                carInDb.VIN = Car.VIN;
                carInDb.Make = Car.Make;
                carInDb.Model = Car.Model;
                carInDb.Style = Car.Style;
                carInDb.Year = Car.Year;
                carInDb.Miles = Car.Miles;
                carInDb.Color = Car.Color;

                await _db.SaveChangesAsync();
            }

            // This is where the magic is that gets to the correct page
            return Redirect("/Cars?userId=" + Car.UserId);
        }



    }
}