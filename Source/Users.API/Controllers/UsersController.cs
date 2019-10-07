using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        /// <param name="mediaType">The media type you wish the response to be returned as</param>
        /// <param name="usersResourceParameters">A resource object containing information about page size and search queries.</param>
        /// <returns>All users with an id, firstname and lastname fields</returns>
        [HttpGet(Name = "GetUsers")]
        [HttpHead]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers(UsersResourceParameters usersResourceParameters, [FromHeader(Name = "Accept")] string mediaType) 
            //For some reason Swagger won't load values into mediaType if FromHeader is used
        {
            if (!_propertyMappingService.ValidMappingExistsFor<UserDto, User>(usersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<UserDto>(usersResourceParameters.Fields))
            {
                return BadRequest();
            }

            var usersFromRepo = _usersRepository.GetUsers(usersResourceParameters);

            var users = _mapper.Map<IEnumerable<UserDto>>(usersFromRepo);

            if (mediaType == "application/jmd.jasonmorsleydev.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = usersFromRepo.TotalCount,
                    pageSize = usersFromRepo.PageSize,
                    currentPage = usersFromRepo.CurrentPage,
                    totalPages = usersFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForUsers(usersResourceParameters, usersFromRepo.HasNext, usersFromRepo.HasPrevious);

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
            }
            else
            {
                var previousPageLink = usersFromRepo.HasPrevious ?
                    CreateUsersResourceUri(usersResourceParameters,
                        ResourceUriType.PreviousPage) : null;

                var nextPageLink = usersFromRepo.HasNext ?
                    CreateUsersResourceUri(usersResourceParameters,
                        ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = usersFromRepo.TotalCount,
                    pageSize = usersFromRepo.PageSize,
                    currentPage = usersFromRepo.CurrentPage,
                    totalPages = usersFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(users.ShapeData(usersResourceParameters.Fields));
            }
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
                case ResourceUriType.Current:
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
        /// <param name="id">The id of the user you want to get</param>
        /// <param name="fields">a list of resource fields you want</param>
        /// <returns>A User with id, firstname and lastname fields</returns>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult GetUser([FromRoute] Guid id,[FromQuery]string fields)
        {
            if (!_typeHelperService.TypeHasProperties<UserDto>
                (fields))
            {
                return BadRequest();
            }

            var userFromRepo = _usersRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userFromRepo);

            var links = CreateLinksForUser(id, fields);

            var linkedResourceToReturn = user.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        /// <summary>
        /// Adds a user given correct input
        /// </summary>
        /// <param name="user">A user populated with required details; firstname and lastname.</param>
        /// <returns>201 Created</returns>
        [HttpPost(Name = "CreateUser")]
        [RequestHeaderMatchesMediaType("Content-Type", new []{
            "application/jmd.jasonmorsleydev.user.full+json",
            "application/jmd.jasonmorsleydev.user.full+xml" })]
        //[RequestHeaderMatchesMediaType("Accept", new[] {"..."} )]
        public ActionResult CreateUser([FromBody] UserForCreationDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userEntity = _mapper.Map<User>(user);

            _usersRepository.AddUser(userEntity);

            if (!_usersRepository.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var userToReturn = _mapper.Map<UserDto>(userEntity);

            var links = CreateLinksForUser(userToReturn.Id, null);

            var linkedResourceToReturn = userToReturn.ShapeData(null)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetUser",
                new { id = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        /// <summary>
        /// Update an user 
        /// </summary>
        /// <param name="userId">The id of the user to update</param>
        /// <param name="userForUpdate">The user with updated values</param>
        /// <returns>An ActionResult of type user</returns>
        /// <response code="422">Validation error</response>
        [HttpPut("{userId}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidationProblemDetails))]
        public ActionResult<User> UpdateUser(
            Guid userId,
            UserForUpdate userForUpdate)
        {
            var userFromRepo = _usersRepository.GetUser(userId);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(userForUpdate, userFromRepo);

            //// update & save
            _usersRepository.UpdateUser(userFromRepo);
            _usersRepository.Save();

            // return the user
            return Ok(_mapper.Map<User>(userFromRepo));
        }

        /// <summary>
        /// Partially update an user
        /// </summary>
        /// <param name="userId">The id of the user you want to get</param>
        /// <param name="patchDocument">The set of operations to apply to the user</param>
        /// <returns>An ActionResult of type user</returns>
        /// <remarks>Sample request (this request updates the user's **first name**)  
        /// 
        /// PATCH /user/userId
        /// [ 
        ///     {
        ///         "op": "replace", 
        ///         "path": "/firstname", 
        ///         "value": "new first name" 
        ///     } 
        /// ] 
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        [HttpPatch("{userId}")]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity,
            Type = typeof(ValidationProblemDetails))]
        public ActionResult<User> UpdateUser(Guid userId, JsonPatchDocument<UserForUpdate> patchDocument)
        {
            var userFromRepo = _usersRepository.GetUser(userId);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            // map to DTO to apply the patch to
            var user = _mapper.Map<UserForUpdate>(userFromRepo);
            patchDocument.ApplyTo(user, ModelState);


            //Manually check the modelstate as badly formed patch documents are not caught by the APIController
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // map the applied changes on the DTO back into the entity
            _mapper.Map(user, userFromRepo);

            // update & save
            _usersRepository.UpdateUser(userFromRepo);
            _usersRepository.Save();

            // return the user
            return Ok(_mapper.Map<User>(userFromRepo));
        }

        /// <summary>
        /// Deletes a user by their userId
        /// </summary>
        /// <param name="userId">The id of the user you wish to delete</param>
        /// <returns>204 NoContent</returns>
        [HttpDelete("{userId}", Name = "DeleteUser")]
        public ActionResult DeleteUser([FromRoute] Guid userId)
        {
            var userFromRepo = _usersRepository.GetUser(userId);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            _usersRepository.DeleteUser(userFromRepo);

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

        private IEnumerable<LinkDto>CreateLinksForUsers(UsersResourceParameters usersResourceParameters, bool hasNext, bool hasPrevious)
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

        [HttpOptions]
        public IActionResult GetUsersOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

    }
}
