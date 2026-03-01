using System;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;

namespace RedGreenBlue.Services.Interfaces;

public interface IGridService
{
    Task<List<Cell>> GetCellsAsync(TeamColor teamColor);
    Task<bool> UpdateCellColorAsync(UpdateCellColorDto newCell);
}
