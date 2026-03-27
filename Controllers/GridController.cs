using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly IGridService _gridService;
        public GridController(IGridService gridService)
        {
            _gridService = gridService;
        }

        [HttpGet("ping")]
        public IActionResult PingGrid()
        {
            return Ok(new { message = "Ping successful" });
        }


        [HttpGet("{teamColor:int}")]
        public async Task<ActionResult<List<Cell>>> GetGrid(TeamColor teamColor)
        {
            if (!Enum.IsDefined(typeof(TeamColor), teamColor))
                return BadRequest("Invalid team color");

            var grid = await _gridService.GetCellsAsync(teamColor);
            return Ok(grid);
        }

        [HttpPut("cell")]
        public async Task<IActionResult> UpdateCellColorAsync([FromBody] UpdateCellColorDto cell)
        {
            if (!Enum.IsDefined(typeof(TeamColor), cell.TeamColor))
            {
                return BadRequest("Invalid team color");
            }

            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin)
            {
                if (!TryGetUserTeam(out var userTeam))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "User team claim is missing or invalid.");
                }

                if (cell.TeamColor != userTeam)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You can only update cells for your own team.");
                }
            }

            try
            {
                var updated = await _gridService.UpdateCellColorAsync(cell);
                if (!updated) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool TryGetUserTeam(out TeamColor userTeam)
        {
            userTeam = default;
            var teamClaim = User.FindFirst("team")?.Value;

            if (string.IsNullOrWhiteSpace(teamClaim))
            {
                return false;
            }

            if (Enum.TryParse<TeamColor>(teamClaim, ignoreCase: true, out var parsedTeam)
                && Enum.IsDefined(typeof(TeamColor), parsedTeam))
            {
                userTeam = parsedTeam;
                return true;
            }

            if (int.TryParse(teamClaim, out var numericTeam)
                && Enum.IsDefined(typeof(TeamColor), numericTeam))
            {
                userTeam = (TeamColor)numericTeam;
                return true;
            }

            return false;
        }


    }
}
