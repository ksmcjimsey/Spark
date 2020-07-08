using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Services
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;


        // One header has a list of details
        public ServiceHeader serviceHeader { get; set; }
        public List<ServiceDetails> serviceDetails { get; set; }


        public DetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }



        public void OnGet(int serviceId)
        {
            // Again we use include to fill in all the data for the id links from
            // the service header.
            serviceHeader = _db.ServiceHeader.Include(s => s.Car).Include(s => s.Car.ApplicationUser).
                FirstOrDefault(s => s.Id == serviceId);

            // Get all the detail records where the service header id matches what was passed in.
            serviceDetails = _db.ServiceDetails.Where(s => s.ServiceHeaderId == serviceId).ToList();
        }
    }
}