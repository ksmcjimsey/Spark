using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

// This code was generated with a new razor page using Entity Framework instead
// of a blank razor page.  The naming was edited to match the rest of the project.
namespace SparkAuto.Pages.ServiceTypes
{
    // When using built in authorization we can use the Authorize keyword
    // Looks in Utility --> SD.cs to see the roles
    [Authorize(Roles = SD.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ServiceType ServiceType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check if id is null and return not found
            if (id == null)
            {
                return NotFound();
            }

            // Get the record form db and put in object
            ServiceType = await _db.ServcieType.FirstOrDefaultAsync(m => m.Id == id);

            // If not found in the DB then return NotFound
            if (ServiceType == null)
            {
                return NotFound();
            }
            return Page();  // Back to page with values filled in by the binding
        }

        // Once the user makes the changes and clicks the Save button the values come back in the
        // Service type due to binding.  The ModelState checks the model requirements against the
        // data sent by the user.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Updates all the properties.
            _db.Attach(ServiceType).State = EntityState.Modified;

            // Another way to update the db value.  Only include fields needed for updating
            // Can get ride of the try / catch below with this method.
            // This way is better for just a few fields that may need to be updated
            //var sericeFromDb = await _db.ServcieType.FirstOrDefaultAsync(s => s.Id == ServiceType.Id);
            //sericeFromDb.Name = ServiceType.Name;
            //sericeFromDb.Price = ServiceType.Price;
            // await _db.SaveChangesAsync();

            // This method updates all the fields and is good for lots of changes.
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(ServiceType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Needed for both methods above
            return RedirectToPage("./Index");
        }

        private bool ServiceTypeExists(int id)
        {
            return _db.ServcieType.Any(e => e.Id == id);
        }
    }
}
