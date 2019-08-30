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
        PagedList<User> GetUsers(UsersResourceParameters usersResourceParameters);
        //IEnumerable<User> GetAll();
        User GetUser(Guid userId);
        //IEnumerable<User> GetUsers(IEnumerable<Guid> userIds);
        void AddUser(User user);
        void DeleteUser(User user);
        void UpdateUser(User user);
        bool UserExists(Guid userId);
        bool Save();
        Task<User> GetUserAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<bool> SaveChangesAsync();
    }
}