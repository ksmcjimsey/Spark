using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

namespace SparkAuto.Pages.ServiceTypes
{
    // When using built in authorization we can use the Authorize keyword
    // Looks in Utility --> SD.cs to see the roles
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        // Need this to find to the form inputs
        // If this object is found on post it is auto passed in
        [BindProperty]
        public ServiceType ServiceType { get; set; }
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // Return back to the page as the page will display a blank form
        public IActionResult OnGet()
        {
            return Page();
        }

        // Posting a ServiceType object - its values are filled in from the submit
        // form and Core3 knows to fill in each object piece from the data sent back.
        // Use bind property instead of passing in the object to the post handler.
        //public async Task<IActionResult> OnPostAsync(ServiceType ServiceObj)
        public async Task<IActionResult> OnPostAsync()
        {
            // If the name and price are not populated then 
            if(!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new service type object to our DB
            //_db.ServcieType.Add(ServiceObj);
            _db.ServcieType.Add(ServiceType);   // Uses bind property so we don't get a passed in object (either option OK)
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");

            
        }
    }
}