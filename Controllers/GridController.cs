using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedGreenBlue.Models;

namespace RedGreenBlue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        [HttpGet("grid/{teamColor}")]
        public async Task<ActionResult<List<Cell>>> GetGrid(TeamColor teamColor)
        {
            var grid = await _gridService.GetGridAsync(teamColor);
            return Ok(grid);
        }
    }
}
