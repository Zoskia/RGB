using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Dtos.User
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public TeamColor Team { get; set; }
    }
}
