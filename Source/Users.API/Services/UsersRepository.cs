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
        //private UserContext _context;

        //public UsersRepository(UserContext context)
        //{
        //    _context = context;
        //}

        private IList<User> _users;

        public UsersRepository()
        {
            _users = new List<User>();
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }

        public User Get(Guid userId)
        {
            return _users.First(x => x.Id == userId);
        }

        public void Add(User user)
        {
            
        }

        public void Delete(Guid userId)
        {
            
        }

        public void Update(User user)
        {
            
        }
    }
}
