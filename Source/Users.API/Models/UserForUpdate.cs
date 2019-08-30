using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Models
{
    /// <summary>
    /// An user for update with FirstName and LastName fields
    /// </summary>
    public class UserForUpdate
    {
        /// <summary>
        /// The first name of the user
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>         
        [Required]
        public string LastName { get; set; }
    }
}
