﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.API.Entities;

namespace Users.API.Services
{
    public interface IUsersRepository
    {
        IEnumerable<User> GetAll();
        User Get(Guid userId);
        //IEnumerable<User> GetUsers(IEnumerable<Guid> userIds);
        void Add(User user);
        void Delete(User user);
        void Update(User user);
        //bool UserExists(Guid userId);
        //bool Save();
    }
}