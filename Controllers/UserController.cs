using Microsoft.AspNetCore.Mvc;
using RedGreenBlue.Dtos;
using RedGreenBlue.Dtos.User;
using RedGreenBlue.Models;
using RedGreenBlue.Services;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        public UserController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(RegisterUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Team = dto.Team
            };

            var newUser = await _authService.RegisterAsync(user);
            if (newUser == null)
                return BadRequest("Username already exists");

            var response = new UserResponseDto
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Team = newUser.Team,
                IsAdmin = newUser.IsAdmin
            };

            return StatusCode(201, response); // TODO: implement CreatedAtAction()
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto dto)
        {
            var user = await _authService.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var token = _jwtService.GenerateToken(user);

            var response = new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Team = user.Team,
                IsAdmin = user.IsAdmin
            };

            return Ok(response);
        }

    }
}
