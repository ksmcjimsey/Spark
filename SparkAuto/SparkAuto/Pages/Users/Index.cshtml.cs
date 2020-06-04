using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Models.ViewModel;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Users
{
    // When using built in authorization we can use the Authorize keyword
    // Looks in Utility --> SD.cs to see the roles
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;


        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // Bind to the view - like angular binding of values to the view
        //[BindProperty]
        //public List<ApplicationUser> ApplicationUserList { get; set; }

        // Use our own viewModel that inlcudes a list of application user objects
        [BindProperty]
        public UsersListViewModel UserListVM { get; set; }


        // Start with page 1 but after that the page is passed in.
        // Because we only use get in the view we need to get the values
        // in the all to OnGet.  The URL arguments come in in the on the
        // method arguements.
        public async Task<IActionResult> OnGet(int productPage=1,
                                               string searchEmail=null,
                                               string searchName=null,
                                               string searchPhone=null
                                              )
        {
            // Step 1: View model packages up the pagination oject the the data list.
            UserListVM = new UsersListViewModel()
            {
                ApplicationUserList = await _db.ApplicationUser.ToListAsync()
            };

            // Step 2: build up pieces for the pagination
            // What page are we adding too
            StringBuilder param = new StringBuilder();
            param.Append("/Users?productPage=:");       // : is replaced in our custom tag helper


            // HOW DOES THIS WORK?
            //   1. The initial time called all the searches are null because there are not get arguemnents
            //   2. Once the user clicks the search button with a search field one or more of these URL parameters are passed
            //   3. Each time a user clicks through the pagination the OnGet is called again so we are making sure the 
            //      last set of URL parameters are reset to the same settings so the search list does not change.
            // Step B: Search - take values passed in on the URL to the OnGet method call and 
            //         add it to the URL
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            // Step C is filter list based on the filter settings
            if (searchEmail!=null)
            {
                UserListVM.ApplicationUserList = await _db.ApplicationUser
                    .Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
            }

            else
            {
                if (searchName != null)
                {
                    UserListVM.ApplicationUserList = await _db.ApplicationUser
                        .Where(u => u.Name.ToLower().Contains(searchName.ToLower())).ToListAsync();
                }

                else
                {
                    if (searchPhone != null)
                    {
                        UserListVM.ApplicationUserList = await _db.ApplicationUser
                            .Where(u => u.PhoneNumber.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
                    }
                }
            }


            // Number of objects in the list (records in the db)
            var count = UserListVM.ApplicationUserList.Count;

            // Step 3: Create the second part of the UserList view model - pagination part
            UserListVM.PagingInfo = new PagingInfo
            {
                // To replace the hard coded "2" i.e. magic string, we can go to Utility/SD.cs
                // and create a constant value there.
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationUserPageSize,
                TotalItems = count,
                UrlParm = param.ToString()
            };

            // Step 4: filter the list to show only the records for the page we need
            //         1st page skip 0, 2nd page skip 2, 3rd page skip 4
            //         (CurrentPage - 1) * ItemsPerPage  start at 1 so 1-1 =0 * 2 start at 0
            UserListVM.ApplicationUserList = UserListVM.ApplicationUserList.OrderBy(p => p.Email)
                .Skip((productPage - 1) * SD.PaginationUserPageSize)
                .Take(SD.PaginationUserPageSize).ToList();

            // Moved into the viewModel above
            //ApplicationUserList = await _db.ApplicationUser.ToListAsync();
            return Page();  // Return to its own page
        }




    }
}