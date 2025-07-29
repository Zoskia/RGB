using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Services.Interfaces;

public interface IGridService
{
    Task<List<Cell>> GetCellsAsync(TeamColor teamColor);
}
