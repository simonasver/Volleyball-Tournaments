using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class TournamentService : ITournamentService
{
    public Tournament AddTeamToTournament(Tournament tournament, Team team)
    {
        tournament.RequestedTeams.Remove(team);
        var gameTeam = new GameTeam()
        {
            Title = team.Title,
            ProfilePicture = team.PictureUrl,
            Description = team.Description,
            Players = new List<GameTeamPlayer>()
        };
        foreach (var teamPlayer in team.Players)
        {
            var gameTeamPlayer = new GameTeamPlayer()
            {
                Name = teamPlayer.Name,
            };
            gameTeam.Players.Add(gameTeamPlayer);
        }

        if (tournament.AcceptedTeams.Count >= tournament.MaxTeams)
        {
            throw new InvalidOperationException("Tournament is already full");
        }
        else
        {
            tournament.AcceptedTeams.Add(gameTeam);
        }

        return tournament;
    }

    public TournamentMatch GenerateEmptyBracket(Tournament tournament, int roundCount)
    {
        var games = new TournamentMatch();
        games = GenerateParentGames(
            tournament,
            new TournamentMatch()
            {
                Tournament = tournament,
                Round = roundCount,
                Game = new Game()
                {
                    Title = tournament.Title + " game " + roundCount,
                    PointsToWin = tournament.PointsToWin,
                    PointDifferenceToWin = tournament.PointDifferenceToWin,
                    MaxSets = tournament.MaxSets,
                    PlayersPerTeam = tournament.PlayersPerTeam,
                    FirstTeamScore = 0,
                    SecondTeamScore = 0,
                    IsPrivate = tournament.IsPrivate,
                    CreateDate = DateTime.Now,
                    LastEditDate = DateTime.Now,
                    Status = GameStatus.New,
                    OwnerId = tournament.OwnerId
                },
            }, roundCount);
        return games;
    }

    private TournamentMatch GenerateParentGames(Tournament tournament, TournamentMatch childMatch, int currentRound)
    {
        if (currentRound <= 0)
        {
            return null;
        }

        childMatch.Parents = new List<TournamentMatch>();

        childMatch.Parents.Add(
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound - 1,
                    Game = new Game()
                    {
                        Title = "Empty " + tournament.Title + " game",
                        PointsToWin = tournament.PointsToWin,
                        PointDifferenceToWin = tournament.PointDifferenceToWin,
                        MaxSets = tournament.MaxSets,
                        PlayersPerTeam = tournament.PlayersPerTeam,
                        FirstTeamScore = 0,
                        SecondTeamScore = 0,
                        IsPrivate = tournament.IsPrivate,
                        CreateDate = DateTime.Now,
                        LastEditDate = DateTime.Now,
                        Status = GameStatus.New,
                        OwnerId = tournament.OwnerId
                    },
                    Child = childMatch
                }, currentRound - 1));

        childMatch.Parents.Add(
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound - 1,
                    Game = new Game()
                    {
                        Title = "Empty " + tournament.Title + " game",
                        PointsToWin = tournament.PointsToWin,
                        PointDifferenceToWin = tournament.PointDifferenceToWin,
                        MaxSets = tournament.MaxSets,
                        PlayersPerTeam = tournament.PlayersPerTeam,
                        FirstTeamScore = 0,
                        SecondTeamScore = 0,
                        IsPrivate = tournament.IsPrivate,
                        CreateDate = DateTime.Now,
                        LastEditDate = DateTime.Now,
                        Status = GameStatus.New,
                        OwnerId = tournament.OwnerId
                    },
                    Child = childMatch
                }, currentRound - 1));

        childMatch.Parents = childMatch.Parents.Where(x => x != null).ToList();

        return childMatch;
    }
    
    public IList<TournamentMatch> PopulateEmptyBrackets(IList<TournamentMatch> tournamentMatches, IList<GameTeam> teams)
    {
        var lowestRound = 1;

        for (int i = 0; i < teams.Count; i = i + 2)
        {
            var match = tournamentMatches.FirstOrDefault(x => x.Round == lowestRound && x.Game.FirstTeam == null && x.Game.SecondTeam == null);
            match.Game.FirstTeam = teams[i];
            if (i + 1 < teams.Count)
            {
                match.Game.SecondTeam = teams[i + 1];
            }

            match.Game = CheckIfGameIsReady(match);
        }

        return tournamentMatches;
    }

    public IEnumerable<TournamentMatch> MoveMatchTeamDown(ICollection<TournamentMatch> tournamentMatches, TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.Game.FirstTeam != null && tournamentMatch.Game.SecondTeam != null)
        {
            throw new InvalidOperationException("Cannot move down a team if it has an opponent");
        }

        if (tournamentMatch.Game.FirstTeam == null && tournamentMatch.Parents.Count > 0 && HasDirectAncestor(tournamentMatch.Parents.OrderBy(x => x.Game.CreateDate).ToList()[0]) || 
            tournamentMatch.Game.SecondTeam == null && tournamentMatch.Parents.Count > 0 && HasDirectAncestor(tournamentMatch.Parents.OrderBy(x => x.Game.CreateDate).ToList()[1]))
        {
            throw new InvalidOperationException("Cannot move down a team that will have an opponent");
        }

        if (tournamentMatch.Game.SecondTeam == null)
        {
            var childMatchOf = FindChildMatchOf(tournamentMatch);
            if (!childMatchOf.Item2)
            {
                if (childMatchOf.Item1.Game.FirstTeam.Title == tournamentMatch.Game.FirstTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.FirstTeam = tournamentMatch.Game.FirstTeam.Copy();
            }
            else
            {
                if (childMatchOf.Item1.Game.SecondTeam.Title == tournamentMatch.Game.FirstTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.SecondTeam = tournamentMatch.Game.FirstTeam.Copy();
            }

            yield return childMatchOf.Item1;
        }
        else if (tournamentMatch.Game.FirstTeam == null)
        {
            var childMatchOf = FindChildMatchOf(tournamentMatch);
            if (!childMatchOf.Item2)
            {
                if (childMatchOf.Item1.Game.FirstTeam.Title == tournamentMatch.Game.SecondTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.FirstTeam = tournamentMatch.Game.SecondTeam.Copy();
            }
            else
            {
                if (childMatchOf.Item1.Game.SecondTeam.Title == tournamentMatch.Game.SecondTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.SecondTeam = tournamentMatch.Game.SecondTeam.Copy();
            }

            yield return childMatchOf.Item1;
        }
    }

    private bool HasDirectAncestor(TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.Parents.Count == 0)
        {
            return (tournamentMatch.Game.FirstTeam != null && tournamentMatch.Game.SecondTeam != null);
        }
        
        return tournamentMatch.Parents.Count != 0 ? (HasDirectAncestor(tournamentMatch.Parents?[0]) || HasDirectAncestor(tournamentMatch.Parents?[1])) : false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tournament"></param>
    /// <param name="tournamentMatch"></param>
    /// <returns>Tournament to update and tournament matches to update</returns>
    public (Tournament, ICollection<TournamentMatch>) MatchesToUpdateInTournamentAfterWonMatch(Tournament tournament, TournamentMatch tournamentMatch)
    {
        var lastRound = tournament.FinalRound;
        List<TournamentMatch> matchesToUpdate = new List<TournamentMatch>();

        if (tournamentMatch.Round == lastRound)
        {
            tournament.Winner = tournamentMatch.Game.Winner;
            tournament.Status = TournamentStatus.Finished;
        }
        else
        {
            var childMatch = FindChildMatchOf(tournamentMatch);
            if (!childMatch.Item2)
            {
                childMatch.Item1.Game.FirstTeam = tournamentMatch.Game.Winner.Copy();
                childMatch.Item1.Game = CheckIfGameIsReady(childMatch.Item1);
                matchesToUpdate.Add(childMatch.Item1);
            }
            else
            {
                childMatch.Item1.Game.SecondTeam = tournamentMatch.Game.Winner.Copy();
                childMatch.Item1.Game = CheckIfGameIsReady(childMatch.Item1);
                matchesToUpdate.Add(childMatch.Item1);
            }
        }

        return (tournament, matchesToUpdate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tournamentMatches"></param>
    /// <param name="parentMatch"></param>
    /// <returns>Child match and from which parent it came</returns>
    private (TournamentMatch, bool) FindChildMatchOf(TournamentMatch parentMatch)
    {
        var parents = parentMatch.Child?.Parents.OrderBy(x => x.Game.CreateDate).ToList();
        if (parents[0] == parentMatch)
        {
            return (parentMatch.Child, false);
        }
        else if(parents[1] == parentMatch)
        {
            return (parentMatch.Child, true);
        }
        

        return (null, false);
    }

    private Game CheckIfGameIsReady(TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.Game.FirstTeam == null || tournamentMatch.Game.SecondTeam == null)
        {
            return tournamentMatch.Game;
        }

        tournamentMatch.Game.Status = GameStatus.Ready;
        tournamentMatch.Game.Title = tournamentMatch.Game.FirstTeam.Title + " vs " + tournamentMatch.Game.SecondTeam.Title;
        tournamentMatch.Game.Description = "Tournament " + tournamentMatch.Tournament.Title + " match in round " + tournamentMatch.Round +
                                           " between " + tournamentMatch.Game.FirstTeam.Title + " vs " + tournamentMatch.Game.SecondTeam.Title;
        return tournamentMatch.Game;
    }
}