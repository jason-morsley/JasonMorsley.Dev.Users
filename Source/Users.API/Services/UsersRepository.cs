using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;
using Users.API.Entities;
using Users.API.Helpers;

namespace Users.API.Services
{
    public class UsersRepository : IUsersRepository
    {
        private UserContext _context;

        public UsersRepository(UserContext context)
        {
            _context = context;
        }

        //private IList<User> _users;
        //public UsersRepository()
        //{
        //    _users = new List<User>();
        //}

        public PagedList<User> GetAll(
            UsersResourceParameters usersResourceParameters)
        {
            var collectionBeforePaging =
                _context.Users
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .AsQueryable();

            return PagedList<User>.Create(collectionBeforePaging,
                usersResourceParameters.PageNumber,
                usersResourceParameters.PageSize);

            //return _users.Users
            //    .OrderBy(x => x.FirstName)
            //    .ThenBy(x => x.LastName)
            //    .ToList();
        }

        public User Get(Guid userId)
        {
            return _context.Users.FirstOrDefault(x => x.Id == userId);
        }

        public void Add(User user)
        {
            //user.Id = Guid.NewGuid();
            _context.Users.Add(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }

        public void Update(User user)
        {
            //no code in this implementation
        }

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(a => a.Id == userId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
