using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{teamColor}")]
        public async Task<ActionResult<List<Cell>>> GetGrid(TeamColor teamColor)
        {
            if (!Enum.IsDefined(typeof(TeamColor), teamColor))
                return BadRequest("Invalid team color");

            var grid = await _gridService.GetCellsAsync(teamColor);
            return Ok(grid);
        }
    }
}
