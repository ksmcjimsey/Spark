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


        // Plain vanilla OnPost
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Add the date to the service header adn fill the shopping cart
                // with a list fo service types.
                CarServiceVM.ServiceHeader.DateAdded = DateTime.Now;
                CarServiceVM.ServiceShoppingCart =
                    _db.ServiceShoppingCart.Include(c => c.ServiceType).ToList();

                // Loop over cart to get the total price
                foreach(var item in CarServiceVM.ServiceShoppingCart)
                {
                    CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
                }

                // Set the service header car Id to the models car ID.
                CarServiceVM.ServiceHeader.CarId = CarServiceVM.Car.Id;

                // Save changes on the service header to the DB
                _db.ServiceHeader.Add(CarServiceVM.ServiceHeader);
                await _db.SaveChangesAsync();

                // Next loop over the details and set them in Service Details
                foreach(var detail in CarServiceVM.ServiceShoppingCart)
                {
                    ServiceDetails serviceDetails = new ServiceDetails
                    {
                        ServiceHeaderId = CarServiceVM.ServiceHeader.Id,
                        ServiceName = detail.ServiceType.Name,
                        ServicePrice = detail.ServiceType.Price,
                        ServiceTypeId = detail.ServiceTypeId
                    };

                    // Add serviceDetails to the DB
                    // Don't call the save chagnes until after the loop is done!
                    _db.ServiceDetails.Add(serviceDetails);
                }

                // Before we save remove all the items from the shopping cart
                // *** Use RemoveRange to remove more then one item ***
                // If passing a list all the records are removed based on primary key
                _db.ServiceShoppingCart.RemoveRange(CarServiceVM.ServiceShoppingCart);

                await _db.SaveChangesAsync();

                return RedirectToPage("../Cars/Index", new { userId = CarServiceVM.Car.UserId });
            }

            // Model not valid
            return Page();
        }


        // Added OnPost and then the value from the razor page button:
        // asp-page-handler="AddToCart".  OnPost + AddToCart
        public async Task<IActionResult> OnPostAddToCart()
        {
            // Service shopping cart consists fo a car id and a service type id
            ServiceShoppingCart objServiceCart = new ServiceShoppingCart()
            {
                CarId = CarServiceVM.Car.Id,    // comes from front end hidden property
                ServiceTypeId = CarServiceVM.ServiceDetails.ServiceTypeId  // Drop down list select is assined to this value
            };

            // Add the shopping card to the database ServiceShoppingCart table
            _db.ServiceShoppingCart.Add(objServiceCart);
            await _db.SaveChangesAsync();

            // Back to the Services create 
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });

        }


        // Remove an item from the cart
        public async Task<IActionResult> OnPostRemoveFromCart(int serviceTypeId)
        {
            // Go to the DB and get me the item from the shopping cart for the car Id 
            // in the bound model and the service type based on service type id.
            ServiceShoppingCart objServiceCart =
                _db.ServiceShoppingCart.FirstOrDefault(s => s.CarId == CarServiceVM.Car.Id 
                    && s.ServiceTypeId == serviceTypeId);

            // Add the shopping card to the database ServiceShoppingCart table
            _db.ServiceShoppingCart.Remove(objServiceCart);
            await _db.SaveChangesAsync();

            // Back to the Services create for the specific car.
            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });

        }
    }
}