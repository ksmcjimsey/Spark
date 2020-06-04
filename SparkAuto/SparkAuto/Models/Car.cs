using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace SparkAuto.Models
{
    // Don't forget to add the the Data --> ApplicationDbContext.cs
    // Steps for new page with multiple DB table access
    // 1. Create new model / db table(s) as needed - use foreign keys to link tables
    // 2. Add the the ApplicationDbContext
    // 3. Run commands to creat the migration and add the table(s)
    // 4. Create a View Model to hold all the models and data for a page
    // 5. Create the page
    // 6. Create subpages and methods to handle data changes and access
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VIN { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public string Style { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public double Miles { get; set; }

        public string Color { get; set; }

        public string UserId { get; set; }

        // Take the property in this model in the attribute and tell it what it 
        // refers to in the other table
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }


    }
}
