using System;
using RedGreenBlue.Models;

namespace RedGreenBlue.Repositories.Interfaces;

public interface IGridRepository
{
    Task<List<Cell>> GetCellsAsync(TeamColor teamColor);
}
