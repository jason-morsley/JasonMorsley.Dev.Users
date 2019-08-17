using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Entities
{
    public static class UserContextExtensions 
    {
        public static void EnsureSeedDataForContext(this UserContext context)
        {
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            var users = new List<User>()
            {
                new User()
                {
                    FirstName = "Jason",
                    LastName = "Morsley",
                },

                new User()
                {
                    Id = new Guid("76053df4-6687-4353-8937-b45556748abe"),
                    FirstName = "John",
                    LastName = "Morsley",
                },

                new User()
                {
                    Id = new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"),
                    FirstName = "George",
                    LastName = "RR Martin"
                },

                new User()
                {
                    Id = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"),
                    FirstName = "Tom",
                    LastName = "Lanoye",
                },

                new User()
                {
                    Id = new Guid("f74d6899-9ed2-4137-9876-66b070553f8f"),
                    FirstName = "Douglas",
                    LastName = "Adams",
                },

                new User()
                {
                    Id = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"),
                    FirstName = "Jens",
                    LastName = "Lapidus",
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
