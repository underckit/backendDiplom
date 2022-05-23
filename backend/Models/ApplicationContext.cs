using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace backend.Models
{
    public class ApplicationContext: DbContext
    {

       
        public DbSet<user> user { get; set; }
        public DbSet<field> field { get; set; }
        public DbSet<ndvi> ndvi { get; set; }
        public DbSet<user_to_field> user_to_field { get; set; }


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных 
        }
       
    }
}
