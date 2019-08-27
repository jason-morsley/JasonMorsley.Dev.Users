using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.API.Models;

namespace Users.API.Controllers
{
    [Route("api")]
    public class RootContoller : Controller
    {
        private IUrlHelper _urlHelper;

        public RootContoller(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Creates links to basic information
        /// </summary>
        /// <param name="mediaType">The media type to return</param>
        /// <returns></returns>
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType) //Again swaggerUI not liking the FromHeader attribute.
        {
            if (mediaType == "application/jmd.jasonmorsleydev.hateoas+json")
            {
                var links = new List<LinkDto>();

                links.Add(
                    new LinkDto(_urlHelper.Link("GetRoot", new { }),
                        "self",
                        "GET"));

                links.Add(
                    new LinkDto(_urlHelper.Link("GetUsers", new { }),
                        "users",
                        "GET"));

                links.Add(
                    new LinkDto(_urlHelper.Link("CreateUser", new { }),
                        "create_user",
                        "POST"));

                return Ok(links);
            }

            return NoContent();
        }
    }
}
