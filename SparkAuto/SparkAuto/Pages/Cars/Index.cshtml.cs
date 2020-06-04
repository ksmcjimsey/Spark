using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models.ViewModel;

namespace SparkAuto.Pages.Cars
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // Added our new view model and bound it to the view so data
        // goes back and forth.
        [BindProperty]
        public CarAndCustomerViewModel CarAndCustVM { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        // Constructor
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // User ID is included for Admin but never for user because the user
        // goes to their car page and does not come from a list of users
        public async Task<IActionResult> OnGet(string userId = null)
        {
            // If the userId is still null the the user is a customer
            if(userId == null)
            {
                // Let's get the user ID - Not sure how this works but gets
                // the logged in users ID somehow
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }

            // Goal is to get a list of the user's cars and to get
            // the users info and put them in a CarAndCustomerViewModel
            // object.  This is availble to the view.
            CarAndCustVM = new CarAndCustomerViewModel()
            {
                // Get a list where the id in the table equals the id of the login
                // user that we got right above.  Convert the return data set to 
                // a list.
                Cars = await _db.Car.Where(c => c.UserId == userId).ToListAsync(),

                // Get the user's data from the User Id we got above.
                UserObj = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId)
            };

            // A way to return back to the page this model is linked to.
            return Page();
        }
    }
}