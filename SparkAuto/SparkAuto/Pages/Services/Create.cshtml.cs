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
using SparkAuto.Models.ViewModel;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Services
{
    // Only available to admin users
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // Bind this value between this back end and the view front end.
        [BindProperty]
        public CarServiceViewModel CarServiceVM { get; set; }


        // db is passed into the constructor using dependency 
        // injection
        public CreateModel(ApplicationDbContext db)
        {
            // Set or local variable to the passed in dependency injection copy
            _db = db;
        }

    

        public async Task<IActionResult> OnGetAsync(int carId)
        {
            // Car Service Model has a car and serice header.
            CarServiceVM = new CarServiceViewModel
            {
                Car = await _db.Car.Include(c => c.ApplicationUser).FirstOrDefaultAsync(c => c.Id == carId),
                ServiceHeader = new Models.ServiceHeader()
            };

            // Get all the service types for this car from the cart
            // Inclucde from shoping cart the service types where the car id matches.
            // Return just the service type name and convert to a list data structure.
            List<String> listServiceTypeInShoppingCart =
                _db.ServiceShoppingCart.Include(c => c.ServiceType)
                                       .Where(c => c.CarId == carId)
                                       .Select(c => c.ServiceType.Name)
                                       .ToList();

            // I think this is getting all the service types that are not already in the cart.
            // Used for the list of services in the drop down.
            IQueryable<ServiceType> listService = from s in _db.ServcieType
                                                  where !(listServiceTypeInShoppingCart.Contains(s.Name))
                                                  select s;

            CarServiceVM.ServiceTypeList = listService.ToList();

            // Get the shopping cart items for our car and convert to a list.
            CarServiceVM.ServiceShoppingCart =
                _db.ServiceShoppingCart.Include(c => c.ServiceType).Where(c => c.CarId == carId).ToList();

            //Add up all the costs
            CarServiceVM.ServiceHeader.TotalPrice = 0;

            foreach(var item in CarServiceVM.ServiceShoppingCart)
            {
                CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
            }

            return Page();

        }
    }
}