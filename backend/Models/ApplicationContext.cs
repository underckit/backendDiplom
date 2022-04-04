using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace backend.Models
{
    public class ApplicationContext: DbContext
    {

       
        public DbSet<User> User { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<Ndvi> Ndvi { get; set; }
        public DbSet<User_to_field> User_to_field { get; set; }


        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
       
    }
}
