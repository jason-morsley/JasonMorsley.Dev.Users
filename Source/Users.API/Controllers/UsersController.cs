﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Users.API.Entities;
using Users.API.Models;
using Users.API.Services;

namespace Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersRepository _usersRepository;
        
        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var usersFromRepo = _usersRepository.GetAll();

            var users = Mapper.Map<IEnumerable<UserDto>>(usersFromRepo);
            return Ok(users);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Guid userId)
        {
            var userFromRepo = _usersRepository.Get(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }
            var user = Mapper.Map<IEnumerable<UserDto>>(userFromRepo);

            return Ok(user);
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userEntity = Mapper.Map<User>(user);

            _usersRepository.Add(user);

            var userToReturn = Mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute("GetUser", new {Id = userToReturn.Id}, userToReturn);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid Id)
        {
            var userFromRepo = _usersRepository.Get(Id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _usersRepository.Delete(userFromRepo);

            return NoContent();
        }
    }
}
