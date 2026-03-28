using System.ComponentModel.DataAnnotations;

namespace RedGreenBlue.Dtos;

public class LoginUserDto
{
    [Required]
    [StringLength(16, MinimumLength = 5)]
    [RegularExpression("^[a-zA-Z0-9._-]{5,16}$")]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(16, MinimumLength = 5)]
    public string Password { get; set; } = null!;
}
