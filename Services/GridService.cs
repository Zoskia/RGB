using System;
using RedGreenBlue.Dtos;
using RedGreenBlue.Helpers;
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

    public async Task<bool> UpdateCellColorAsync(UpdateCellColorDto newCell)
    {
        ValidateColorSpectrum(newCell);
        return await _gridRepository.UpdateCellColorAsync(newCell);
    }

    private static void ValidateColorSpectrum(UpdateCellColorDto cell)
    {
        var isCorrectSpectrum = cell.TeamColor switch
        {
            TeamColor.Red => ColorHelper.IsRed(cell.HexColor),
            TeamColor.Green => ColorHelper.IsGreen(cell.HexColor),
            TeamColor.Blue => ColorHelper.IsBlue(cell.HexColor),
            _ => throw new ArgumentOutOfRangeException(nameof(cell.TeamColor), "Unsupported team color.")
        };

        if (!isCorrectSpectrum)
        {
            throw new InvalidOperationException("Wrong color spectrum");
        }
    }
}
