using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;
using Users.API.Entities;

namespace Users.API.Services
{
    public class UsersRepository : IUsersRepository
    {
        private UserContext _users;

        public UsersRepository(UserContext context)
        {
            _users = context;
        }

        //private IList<User> _users;
        //public UsersRepository()
        //{
        //    _users = new List<User>();
        //}

        public IEnumerable<User> GetAll()
        {
            return _users.Users
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
        }

        public User Get(Guid userId)
        {
            return _users.Users.First(x => x.Id == userId);
        }

        public void Add(User user)
        {
            //user.Id = Guid.NewGuid();
            _users.Users.Add(user);
        }

        public void Delete(User user)
        {
            _users.Users.Remove(user);
        }

        public void Update(User user)
        {
            //no code in this implementation
        }

        public bool UserExists(Guid userId)
        {
            return _users.Users.Any(a => a.Id == userId);
        }

        public bool Save()
        {
            return (_users.SaveChanges() >= 0);
        }
    }
}
