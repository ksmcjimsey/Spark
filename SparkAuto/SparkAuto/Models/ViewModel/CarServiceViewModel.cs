using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models.ViewModel
{
    // This view model is used to pass data back and forth to the 
    // service view.
    public class CarServiceViewModel
    {
        // Car that the service is done on
        public Car Car { get; set; }

        // Service Header and details for this specific service
        public ServiceHeader ServiceHeader { get; set; }
        public ServiceDetails ServiceDetails { get; set; }

        // List of all services
        public List<ServiceType> ServiceTypeList { get; set; }

        // List of services in the cart
        public List<ServiceShoppingCart> ServiceShoppingCart { get; set; }
    }
}
