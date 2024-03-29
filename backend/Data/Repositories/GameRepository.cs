﻿using Backend.Data.Entities.Game;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _dbContext;

    public GameRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<IEnumerable<Game>> GetAllAsync(bool all, SearchParameters searchParameters)
    {
        var queryable = _dbContext.Games.AsQueryable().Where(x => x.TournamentMatch == null);
        if (!all)
        {
            queryable = queryable.Where(x => !x.IsPrivate);
        }

        queryable = queryable.Where(x => x.Title.Contains(searchParameters.SearchInput)).OrderByDescending(x => x.CreateDate)
            .Include(x => x.TournamentMatch)
            .Include(x => x.Winner)
            .Include(x => x.FirstTeam)
            .Include(x => x.SecondTeam);
        return await PagedList<Game>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<IEnumerable<Game>> GetAllUserAsync(SearchParameters searchParameters, string userId)
    {
        var queryable = _dbContext.Games.Where(x => x.Title.Contains(searchParameters.SearchInput)).AsQueryable().OrderByDescending(x => x.CreateDate)
            .Where(x => x.OwnerId == userId).Where(x => x.TournamentMatch == null)
            .Include(x => x.Winner)
            .Include(x => x.FirstTeam)
            .Include(x => x.SecondTeam);
        return await PagedList<Game>.CreateAsync(queryable, searchParameters.PageNumber, searchParameters.PageSize);
    }

    public async Task<Game?> GetAsync(Guid gameId)
    {
        return await _dbContext.Games.
            Include(x => x.RequestedTeams)
                .ThenInclude(x => x.Players)
            .Include(x => x.FirstTeam)
                .ThenInclude(x => x!.Players)
            .Include(x => x.SecondTeam)
                .ThenInclude(x => x!.Players)
            .Include(x => x.Sets)
                .ThenInclude(x => x.Players)
            .Include(x => x.Winner)
            .Include(x => x.TournamentMatch)
                .ThenInclude(x => x!.Tournament)
            .Include(x => x.Managers)
            .FirstOrDefaultAsync(x => x.Id == gameId);
    }

    public async Task<Game> CreateAsync(Game game)
    {
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task<Game> UpdateAsync(Game game)
    {
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task DeleteAsync(Guid gameId)
    {
        var gameToDelete = await _dbContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
        if (gameToDelete != null)
        {
            _dbContext.Games.Remove(gameToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}