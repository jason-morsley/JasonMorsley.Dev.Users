using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UsersController : Controller
    {
        private IUsersRepository _usersRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;
        private IMapper _mapper;

        public UsersController(IUsersRepository usersRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IUrlHelper urlHelper,
            ITypeHelperService typeHelperService)
        {
            _usersRepository = usersRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _mapper = mapper;
            _typeHelperService = typeHelperService;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>All users with an id, firstname and lastname fields</returns>
        [HttpGet(Name = "GetUsers")]
        public ActionResult GetAll(UsersResourceParameters usersResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<UserDto, User>
                (usersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<UserDto>
                (usersResourceParameters.Fields))
            {
                return BadRequest();
            }

            var usersFromRepo = _usersRepository.GetAll(usersResourceParameters);

            //var previousPageLink = usersFromRepo.HasPrevious ?
            //    CreateUsersResourceUri(usersResourceParameters,
            //        ResourceUriType.PreviousPage) : null;

            //var nextPageLink = usersFromRepo.HasNext ?
            //    CreateUsersResourceUri(usersResourceParameters,
            //        ResourceUriType.NextPage) : null;

            var paginationMetaData = new //https://i.imgur.com/QTQgOKu.png
            {
                totalCount = usersFromRepo.TotalCount,
                pageSize = usersFromRepo.PageSize,
                currentPage = usersFromRepo.CurrentPage,
                totalPages = usersFromRepo.TotalPages,
                //nextPageLink = nextPageLink,
                //previousPageLink = previousPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetaData));

            var links = CreateLinksForUsers(usersResourceParameters, usersFromRepo.HasNext, usersFromRepo.HasPrevious);

            var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

            var shapedUsers = users.ShapeData(usersResourceParameters.Fields);

            var shapedUsersWithLinks = shapedUsers.Select(user =>
            {
                var userAsDictionary = user as IDictionary<string, object>;
                var userLinks = CreateLinksForUser((Guid)userAsDictionary["Id"], usersResourceParameters.Fields);
                userAsDictionary.Add("links", userLinks);

                return userAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedUsersWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);

            //var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);
            //return Ok(users.ShapeData(usersResourceParameters.Fields));
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
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
                            searchQuery = usersResourceParameters.SearchQuery,
                            pageNumber = usersResourceParameters.PageNumber - 1,
                            pageSize = usersResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
                            searchQuery = usersResourceParameters.SearchQuery,
                            pageNumber = usersResourceParameters.PageNumber + 1,
                            pageSize = usersResourceParameters.PageSize
                        });

                default:
                    return _urlHelper.Link("GetUsers",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
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
        /// <param name="fields">a list of resource fields you want</param>
        /// <returns>A User with id, firstname and lastname fields</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{userId}", Name = "GetUser")]
        public ActionResult Get([FromRoute] Guid userId,[FromQuery]string fields)
        {
            if (!_typeHelperService.TypeHasProperties<UserDto>
                (fields))
            {
                return BadRequest();
            }

            var userFromRepo = _usersRepository.Get(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userFromRepo);

            //var links = CreateLinksForUser(userId, fields);

            var linkedResourceToReturn = user.ShapeData(fields)
                as IDictionary<string, object>;

            //linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
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
        public void Put([FromRoute] Guid userId, [FromBody] User user)
        {
            _usersRepository.Update(user);
        }

        /// <summary>
        /// Deletes a user by their userId
        /// </summary>
        /// <param name="userId">The id of the user you wish to delete</param>
        /// <returns>204 NoContent</returns>
        [HttpDelete("{userId}", Name = "DeleteUser")]
        public ActionResult Delete([FromRoute] Guid userId)
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

        private IEnumerable<LinkDto> CreateLinksForUser(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetUser", new { id = id }),
                        "self",
                        "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetUser", new { id = id, fields = fields }),
                        "self",
                        "GET"));
            }

            links.Add(
                new LinkDto(_urlHelper.Link("DeleteUser", new { id = id }),
                    "delete_user",
                    "DELETE"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForUsers(
            UsersResourceParameters usersResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
                new LinkDto(CreateUsersResourceUri(usersResourceParameters,
                        ResourceUriType.Current)
                    , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateUsersResourceUri(usersResourceParameters,
                            ResourceUriType.NextPage),
                        "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateUsersResourceUri(usersResourceParameters,
                            ResourceUriType.PreviousPage),
                        "previousPage", "GET"));
            }

            return links;
        }
    }
}
