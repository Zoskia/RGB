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
    [RegularExpression("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$", ErrorMessage = "Invalid HEX color")]
    public string HexColor { get; set; } = "#CCCCCC";

    [Required]
    public TeamColor TeamColor { get; set; }
}
