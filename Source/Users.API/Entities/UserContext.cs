using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace Users.API.Entities
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            //Database.Migrate();
        }

        public DbSet<User> Users { get; set; }
    }
}
