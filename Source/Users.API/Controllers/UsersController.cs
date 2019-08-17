using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.API.Entities;
using Users.API.Models;
using Users.API.Services;

namespace Users.API.Controllers
{
    [Route("api/users")]
    //[ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>All users with an id, firstname and lastname fields</returns>
        [HttpGet]
        public ActionResult Get()
        {
            var usersFromRepo = _usersRepository.GetAll();

            var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);
            return Ok(users);
        }

        /// <summary>
        /// Get a user by their id
        /// </summary>
        /// <param name="userId">The id of the user you want to get</param>
        /// <returns>A User with id, firstname and lastname fields</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult Get(Guid userId)
        {
            var userFromRepo = _usersRepository.Get(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<IEnumerable<UserDto>>(userFromRepo);
            return Ok(user);
        }

        /// <summary>
        /// Adds a user given correct input
        /// </summary>
        /// <param name="user">A user populated with required details; firstname and lastname.</param>
        /// <returns>201 Created</returns>
        [HttpPost(Name = "CreateUser")]
        public ActionResult Post([FromBody] UserForCreationDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userEntity = _mapper.Map<User>(user);

            _usersRepository.Add(userEntity);

            if (!_usersRepository.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var userToReturn = _mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute("GetUser",
                new {Id = userToReturn.Id},
                userToReturn);
        }
        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }
        /// <summary>
        /// Deletes a user by their id
        /// </summary>
        /// <param name="Id">The id of the user you wish to delete</param>
        /// <returns>204 NoContent</returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(Guid Id)
        {
            var userFromRepo = _usersRepository.Get(Id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _usersRepository.Delete(userFromRepo);

            if (!_usersRepository.Save())
            {
                throw new Exception($"Deleting user {Id} failed on save");
            }

            return NoContent();
        }
    }
}
