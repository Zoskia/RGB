using System.ComponentModel.DataAnnotations;

namespace RedGreenBlue.Models;

public class Cell
{
    public int Id { get; set; }

    [Required]
    [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid HEX color")]
    public string HexColor { get; set; } = "#CCCCCC";

    public int Q { get; set; }
    public int R { get; set; }

    [Required]
    public TeamColor TeamColor { get; set; }

}