using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Entities
{
    /// <summary>
    /// A user with Id, FirstName and LastName fields
    /// </summary>
    public class User
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The first name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        //[Required]
        //public DateTimeOffset DateOfBirth { get; set; }
    }
}
