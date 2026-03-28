using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedGreenBlue.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 5)]
    [RegularExpression("^[a-zA-Z0-9._-]{5,16}$")]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string Salt { get; set; } = null!;

    [Required]
    public TeamColor Team { get; set; }

    public bool IsAdmin { get; set; } = false;
}
