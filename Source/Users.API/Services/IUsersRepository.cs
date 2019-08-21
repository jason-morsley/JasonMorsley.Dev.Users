using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Entities;
using Users.API.Helpers;

namespace Users.API.Services
{
    public interface IUsersRepository
    {
        PagedList<User> GetAll(UsersResourceParameters usersResourceParameters);
        //IEnumerable<User> GetAll();
        User Get(Guid userId);
        //IEnumerable<User> GetUsers(IEnumerable<Guid> userIds);
        void Add(User user);
        void Delete(User user);
        void Update(User user);
        bool UserExists(Guid userId);
        bool Save();
    }
}