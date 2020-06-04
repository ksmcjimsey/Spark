using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Cars
{
    public class CreateModel : PageModel
    {
        // Need this to find to the form inputs
        // If this object is found on post it is auto passed in
        [BindProperty]
        public Car Car { get; set; }
        private readonly ApplicationDbContext _db;

        // This type of data can be added to the view before passed back
        // to the users request.
        [TempData]
        public string StatusMessage { get; set; }


        // Constructor
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // Passing in a user ID so we can use it to find the user's cars.
        public IActionResult OnGet(string userId=null)
        {
            Car = new Car();

            // If the userId is still null the the user is a customer
            if (userId == null)
            {
                // Let's get the user ID - Not sure how this works but gets
                // the logged in users ID somehow
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;   // Get the ID so we can use it to look up cars.
            }
            Car.UserId = userId;
            return Page();
        }

        
        // Run after the user fills in the "Add New Car" form and clicks the
        // the "Create" button.
        public async Task<IActionResult> OnPostAsync()
        {
            // If the form / object requirements are not met then 
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new car  object to our DB
            _db.Car.Add(Car);
            await _db.SaveChangesAsync();

            StatusMessage = "Car has been added successfully";

            // We need to send the user Id of the user.  If the admin adds the car
            // then we don't want the admin users cars but the other users
            return RedirectToPage("Index", new { userId = Car.UserId } );


        }
    }
}