using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.API.Entities;
using Users.API.Helpers;
using Users.API.Models;
using Users.API.Services;

namespace Users.API.Controllers
{
    [Route("api/users")]
    //[ApiController]
    public class UsersController : Controller
    {
        private IUsersRepository _usersRepository;
        private IUrlHelper _urlHelper;
        private IMapper _mapper;

        public UsersController(IUsersRepository usersRepository,
            IMapper mapper,
            IUrlHelper urlHelper)
        {
            _usersRepository = usersRepository;
            _urlHelper = urlHelper;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>All users with an id, firstname and lastname fields</returns>
        [HttpGet(Name = "GetUsers")]
        public ActionResult GetAll(UsersResourceParameters usersResourceParameters)
        {
            var usersFromRepo = _usersRepository.GetAll(usersResourceParameters);

            var previousPageLink = usersFromRepo.HasPrevious ? 
                CreateUsersResourceUri(usersResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = usersFromRepo.HasNext ?
                CreateUsersResourceUri(usersResourceParameters, 
                ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = usersFromRepo.TotalCount,
                pageSize = usersFromRepo.PageSize,
                currentPage = usersFromRepo.CurrentPage,
                totalPages = usersFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));


            //var usersFromRepo = _usersRepository.GetAll();

            var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);
            return Ok(users);
        }

        private string CreateUsersResourceUri(
            UsersResourceParameters usersResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = usersResourceParameters.SearchQuery,
                            pageNumber = usersResourceParameters.PageNumber - 1,
                            pageSize = usersResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = usersResourceParameters.SearchQuery,
                            pageNumber = usersResourceParameters.PageNumber + 1,
                            pageSize = usersResourceParameters.PageSize
                        });

                default:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            searchQuery = usersResourceParameters.SearchQuery,
                            pageNumber = usersResourceParameters.PageNumber,
                            pageSize = usersResourceParameters.PageSize
                        });
            }
        }

        /// <summary>
        /// Get a user by their id
        /// </summary>
        /// <param name="userId">The id of the user you want to get</param>
        /// <returns>A User with id, firstname and lastname fields</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{userId}", Name = "GetUser")]
        public ActionResult Get([FromRoute]Guid userId)
        {
            var userFromRepo = _usersRepository.Get(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userFromRepo);
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
        /// <param name="userId"></param>
        /// <param name="user"></param>
        [HttpPut("{userId}")]
        public void Put([FromRoute]Guid userId, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }

        /// <summary>
        /// Deletes a user by their userId
        /// </summary>
        /// <param name="userId">The id of the user you wish to delete</param>
        /// <returns>204 NoContent</returns>
        [HttpDelete("{userId}")]
        public ActionResult Delete([FromRoute]Guid userId)
        {
            var userFromRepo = _usersRepository.Get(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _usersRepository.Delete(userFromRepo);

            if (!_usersRepository.Save())
            {
                throw new Exception($"Deleting user {userId} failed on save");
            }

            return NoContent();
        }
    }
}
