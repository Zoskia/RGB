using System.ComponentModel.DataAnnotations;
using RedGreenBlue.Models;

namespace RedGreenBlue.Dtos.User
{
    public class RegisterUserDto
    {
        [Required]
        [StringLength(16, MinimumLength = 5)]
        [RegularExpression("^[a-zA-Z0-9._-]{5,16}$")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(16, MinimumLength = 5)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Team is required.")]
        [EnumDataType(typeof(TeamColor), ErrorMessage = "Invalid team color.")]
        public TeamColor? Team { get; set; }
    }
}
