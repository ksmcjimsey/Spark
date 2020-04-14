using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SparkAuto.Pages.ServiceTypes
{
    public class CreateModel : PageModel
    {
        // Return back to the page as the page will display a blank form
        public IActionResult OnGet()
        {
            return Page();
        }

        // 
        public IActionResult OnPost()
        {
            return Page();
        }
    }
}