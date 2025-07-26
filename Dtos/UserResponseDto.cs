using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Dtos.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public TeamColor Team { get; set; }
        public bool isAdmin { get; set; }
    }
}
