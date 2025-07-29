using System;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories;
using RedGreenBlue.Repositories.Interfaces;
using RedGreenBlue.Services.Interfaces;

namespace RedGreenBlue.Services;

public class GridService : IGridService
{
    private readonly IGridRepository _gridRepository;
    public GridService(IGridRepository gridRepository)
    {
        _gridRepository = gridRepository;
    }

    public async Task<List<Cell>> GetCellsAsync(TeamColor teamColor)
    {
        return await _gridRepository.GetCellsAsync(teamColor);
    }
}
