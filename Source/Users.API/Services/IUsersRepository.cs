using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Services
{
    public interface IUsersRepository
    {
        IEnumerable<User> GetAllUsers();
        User GetUser(Guid userId);
        //IEnumerable<User> GetUsers(IEnumerable<Guid> userIds);
        void AddUser(User user);
        void DeleteUser(User user);
        void UpdateUser(User user);
        //bool UserExists(Guid userId);
        //bool Save();
    }
}