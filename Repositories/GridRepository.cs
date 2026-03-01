using System;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Data;
using RedGreenBlue.Dtos;
using RedGreenBlue.Models;
using RedGreenBlue.Repositories.Interfaces;

namespace RedGreenBlue.Repositories;

public class GridRepository : IGridRepository
{
    private readonly ApplicationDbContext _context;
    public GridRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Cell>> GetCellsAsync(TeamColor teamColor)
    {
        return _context.Cells.Where(c => c.TeamColor == teamColor).ToListAsync();
    }

    public async Task<bool> UpdateCellColorAsync(UpdateCellColorDto newCell)
    {
        var oldCell = await _context.Cells
            .FirstOrDefaultAsync(c => c.Q == newCell.Q && c.R == newCell.R && c.TeamColor == newCell.TeamColor);

        if (oldCell == null) return false;

        oldCell.HexColor = newCell.HexColor;
        await _context.SaveChangesAsync();

        return true;
    }

}
