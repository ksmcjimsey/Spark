using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }


        public async Task<IActionResult> OnGetAsync(string email)
        {
            // Check if the ID (email) is null and if so return the not found error
            if (email == null)
            {
                return NotFound();
            }

            // Get the record form db and put in object
            ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == email);


            return Page();  // Back to page with values filled in by the binding
        }


        // Create an OnPost method to handle the update from the admin user on
        // the user account.
        public async Task<IActionResult> OnPostAsync()
        {
            // Check that the edit page submitted a valid model
            if(!ModelState.IsValid)
            {
                return Page();  // Not valid return to the page
            }

            // Updated all the properties - Not sure what this does
            _db.Attach(ApplicationUser).State = EntityState.Modified;

            // Save the changes and catch any error
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(ApplicationUser.Email)) {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Go back to the User Index page
            return RedirectToPage("./Index");

        }


        // Helper method that returns true if the email is found in 
        // the DB
        private bool UserExists (string email)
        {
            return _db.Users.Any(e => e.Email == email);
        }



    }
}