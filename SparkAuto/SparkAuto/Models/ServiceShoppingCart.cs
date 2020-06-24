using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models
{
    // User can add auto services to the cart
    public class ServiceShoppingCart
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int ServiceTypeId { get; set; }

        // I think this gets the car based on the car Id.
        [ForeignKey("CarId")]
        public virtual Car Car { get; set; }

        // I think this loads in the service type from the serviceType
        // Id.
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }

        }
}
