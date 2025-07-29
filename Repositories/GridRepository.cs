using System;
using Microsoft.EntityFrameworkCore;
using RedGreenBlue.Data;
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
}
