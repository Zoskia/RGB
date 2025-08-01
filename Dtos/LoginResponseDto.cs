using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public TeamColor Team { get; set; }
    public bool IsAdmin { get; set; }
}
