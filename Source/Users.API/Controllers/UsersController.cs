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
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersController(IUsersRepository usersRepository, IMapper mapper)
        {
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

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
        [HttpGet("{id}")]
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

        [HttpPost]
        public ActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userEntity = _mapper.Map<User>(user);

            _usersRepository.Add(user);

            var userToReturn = _mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute("GetUser",
                new {Id = userToReturn.Id},
                userToReturn);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }

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
