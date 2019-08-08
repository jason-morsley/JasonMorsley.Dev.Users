using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.API.Services;

namespace Users.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersRepository _usersRepository;
        
        public UsersController()
        {
            _usersRepository = new UsersRepository();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            var users = _usersRepository.GetAll();
            return Ok(users);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Guid userId)
        {
            var user = _usersRepository.Get(userId);
            return Ok(user);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] User user)
        {
            _usersRepository.Add(user);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(Guid userId)
        {
            _usersRepository.Delete(userId);
        }
    }
}
