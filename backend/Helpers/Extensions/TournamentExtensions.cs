﻿using System.Numerics;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Tournament;

namespace Backend.Helpers.Extensions;

public static class TournamentExtensions
{
    public static int CountTournamentRounds(this ICollection<GameTeam> acceptedTeams)
    {
        var teamCount = acceptedTeams.Count;
        var closestPowerOf2 = BitOperations.RoundUpToPowerOf2((uint)teamCount);
        return (int)Math.Log2(closestPowerOf2);
    }

    public static ICollection<TournamentMatch> ToList(this TournamentMatch tournamentMatches)
    {
        var tournamentMatchesList = new List<TournamentMatch>();
        AddTournamentMatchesToList(tournamentMatches, tournamentMatchesList);
        return tournamentMatchesList;
    }

    private static void AddTournamentMatchesToList(TournamentMatch? currentMatch,
        List<TournamentMatch> tournamentMatchesList)
    {
        if (currentMatch == null)
        {
            return;
        }
        else
        {
            tournamentMatchesList.Add(currentMatch);
            AddTournamentMatchesToList(currentMatch.FirstParent, tournamentMatchesList);
            AddTournamentMatchesToList(currentMatch.SecondParent, tournamentMatchesList);
        }
    }
}