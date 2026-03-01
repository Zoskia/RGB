using System;
using System.ComponentModel.DataAnnotations;
using RedGreenBlue.Models;

namespace RedGreenBlue.Dtos;

public class UpdateCellColorDto
{
    [Required]
    public int Q { get; set; }

    [Required]
    public int R { get; set; }

    [Required]
    [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid HEX color")]
    public string HexColor { get; set; } = "#CCCCCC";

    [Required]
    public TeamColor TeamColor { get; set; }
}
