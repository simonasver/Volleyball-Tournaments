﻿using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class TournamentMatchRepository : ITournamentMatchRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public TournamentMatchRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    

    public async Task<IList<TournamentMatch>> GetAllTournamentAsync(Guid tournamentId, bool allData)
    {
        if (allData)
        {
            return await _dbContext.TournamentMatches
            .Include(x => x.Tournament)
            .Include(x => x.FirstParent)
            .Include(x => x.SecondParent)
            .Include(x => x.Game)
                .ThenInclude(x => x.FirstTeam)
                    .ThenInclude(x => x!.Players)
            .Include(x => x.Game)
                .ThenInclude(x => x.SecondTeam)
                    .ThenInclude(x => x!.Players)
            .Where(x => x.Tournament.Id == tournamentId)
            .ToListAsync();
        }
        else
        {
            return await _dbContext.TournamentMatches
                .Include(x => x.Game)
                .Include(x => x.Game.FirstTeam)
                .Include(x => x.Game.SecondTeam)
                .Where(x => x.Tournament.Id == tournamentId)
                .Select(x => new TournamentMatch()
                {
                    Id = x.Id,
                    Round = x.Round,
                    ThirdPlace = x.ThirdPlace,
                    Game = x.Game,
                    FirstParentId = x.FirstParentId,
                    SecondParentId = x.SecondParentId,
                    TournamentId = x.TournamentId
                })
                .ToListAsync();
        }
    }

    public async Task<TournamentMatch> UpdateAsync(TournamentMatch tournamentMatch)
    {
        _dbContext.TournamentMatches.Update(tournamentMatch);
        await _dbContext.SaveChangesAsync();
        return tournamentMatch;
    }
}