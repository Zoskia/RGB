using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
