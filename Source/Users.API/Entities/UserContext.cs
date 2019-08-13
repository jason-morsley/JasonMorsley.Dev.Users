//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;

//namespace Users.API.Entities
//{
//    public class UserContext : DbContext
//    {
//        public UserContext(DbContextOptions<UserContext> options) : base(options)
//        {
//            Database.Migrate();
//        }

//        public DbSet<User> Users { get; set; }
//    }
//}
