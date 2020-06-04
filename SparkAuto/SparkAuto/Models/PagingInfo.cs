using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models
{
    public class PagingInfo
    {
        // Hand made pagination 
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        // Property that is based on calculations of other properties
        // instead of using getters and setters
        // Example 35 entries at 10 per page = 3 or 3.5 decimal --> ceiling = 4
        public int TotalPage => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);

        public String UrlParm { get; set; }
    }
}
