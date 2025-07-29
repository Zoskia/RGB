using Microsoft.AspNetCore.Mvc;
using RedGreenBlue.Dtos;
using RedGreenBlue.Dtos.User;
using RedGreenBlue.Models;
using RedGreenBlue.Services;

namespace RedGreenBlue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        public UserController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<ActionResult<UserResponseDto>> Login(LoginUserDto dto)
        {
            var user = await _authService.LoginAsync(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Team = user.Team,
                IsAdmin = user.IsAdmin
            };

            return Ok(response);
        }

    }
}
