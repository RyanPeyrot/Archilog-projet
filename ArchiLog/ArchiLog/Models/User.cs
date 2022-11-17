using ArchiLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArchiLog.Models
{
    public class User : BaseModel
    {
        [StringLength(50, MinimumLength = 5)]
        [Required()]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 7)]
        public string Password { get; set; }


    }
}
