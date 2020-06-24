using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Models;

namespace SparkAuto.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Don't forget to add migration and update database from the
        // Package Manager Console
        public DbSet<ServiceType> ServcieType { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<Car> Car { get; set; }

        public DbSet<ServiceShoppingCart> ServiceShoppingCart { get; set; }
        public DbSet<ServiceHeader> ServiceHeader { get; set; }
        public DbSet<ServiceDetails> ServiceDetails { get; set; }

    }
}
