using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.ServiceTypes
{
    public class IndexModel : PageModel
    {
        // Create a local DB context
        private readonly ApplicationDbContext _db;

        // Store a list of service type objects
        public IList<ServiceType> ServiceType { get; set; }


        // Constructor - db comes in using dependency injection
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> OnGet()
        {
            ServiceType = await _db.ServcieType.ToListAsync();
            return Page();
        }
    }
}