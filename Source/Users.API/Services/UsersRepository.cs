using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;
using Users.API.Entities;
using Users.API.Helpers;
using System.Linq.Dynamic.Core;
using Users.API.Models;

namespace Users.API.Services
{
    public class UsersRepository : IUsersRepository
    {
        private UserContext _context;
        private IPropertyMappingService _propertyMappingService;

        public UsersRepository(UserContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        //private IList<User> _users;
        //public UsersRepository()
        //{
        //    _users = new List<User>();
        //}

        public PagedList<User> GetAll(
            UsersResourceParameters usersResourceParameters)
        {
            //var collectionBeforePaging = _context.Users
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging = 
                _context.Users.ApplySort(usersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<UserDto, User>());

            if (!string.IsNullOrEmpty(usersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = usersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause) ||
                                a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<User>.Create(collectionBeforePaging,
                usersResourceParameters.PageNumber,
                usersResourceParameters.PageSize);
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
