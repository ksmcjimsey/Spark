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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }



        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the User based on the ID
            ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == id);

            // Check that we found the user we are looking to remove.
            if (ApplicationUser == null)
            {
                return NotFound();
            }

            // Load the delete page now with the ApplicationUser data filled in.
            return Page();
        }


        // The data for this record is in a post but we can still take
        // in parts of it in the arguments like we do the ID here.
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == id);

            // If we got the user reference ojbect back from our search then
            // we cna remove it.
            if (ApplicationUser != null)
            {
                _db.ApplicationUser.Remove(ApplicationUser);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}