using System;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;

namespace RedGreenBlue.Repositories.Interfaces;

public interface IGridRepository
{
    Task<List<Cell>> GetCellsAsync(TeamColor teamColor);

    Task<bool> UpdateCellColorAsync(UpdateCellColorDto cell);
}
