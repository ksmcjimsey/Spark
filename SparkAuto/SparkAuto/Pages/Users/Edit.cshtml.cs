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
            if (!ModelState.IsValid)
            {
                return Page();  // Not valid return to the page
            }

            else
            {
                var userInDb = await _db.ApplicationUser.SingleOrDefaultAsync
                    (u => u.Id == ApplicationUser.Id);

                if (userInDb == null)
                {
                    return NotFound();
                }

                userInDb.Name = ApplicationUser.Name;
                userInDb.PhoneNumber = ApplicationUser.PhoneNumber;
                userInDb.Address = ApplicationUser.Address;
                userInDb.City = ApplicationUser.City;
                userInDb.PostalCode = ApplicationUser.PostalCode;

                await _db.SaveChangesAsync();
                return RedirectToPage("Index");
            }

        }

    }

}