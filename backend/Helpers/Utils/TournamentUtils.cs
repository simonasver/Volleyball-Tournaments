using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;

namespace Backend.Helpers.Utils;

public class TournamentUtils
{
    public static Tournament AddTeamToTournament(Tournament tournament, Team team)
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

    public static TournamentMatch GenerateEmptyBracket(Tournament tournament, int roundCount)
    {
        var games = new TournamentMatch();
        games = GenerateParentGames(
            tournament,
            new TournamentMatch()
            {
                Tournament = tournament,
                Round = roundCount,
                ThirdPlace = false,
                Game = new Game()
                {
                    Title = tournament.Title + " game " + roundCount,
                    Basic = tournament.Basic,
                    PointsToWin = tournament.PointsToWin,
                    PointsToWinLastSet = tournament.PointsToWinLastSet,
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

    public static TournamentMatch GenerateThirdPlaceMatch(Tournament tournament)
    {
        return new TournamentMatch()
        {
            Tournament = tournament,
            Round = 0,
            ThirdPlace = true,
            Game = new Game()
            {
                Title = tournament.Title + " game " + "third place",
                Basic = tournament.Basic,
                PointsToWin = tournament.PointsToWin,
                PointsToWinLastSet = tournament.PointsToWinLastSet,
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
        };
    }

    private static TournamentMatch GenerateParentGames(Tournament tournament, TournamentMatch childMatch, int currentRound)
    {
        if (currentRound <= 0)
        {
            return null;
        }

        childMatch.FirstParent = (
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound - 1,
                    Game = new Game()
                    {
                        Title = "Empty " + tournament.Title + " game",
                        Basic = tournament.Basic,
                        PointsToWin = tournament.PointsToWin,
                        PointsToWinLastSet = tournament.PointsToWinLastSet,
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
                }, currentRound - 1));

        childMatch.SecondParent = (
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound - 1,
                    Game = new Game()
                    {
                        Title = "Empty " + tournament.Title + " game",
                        Basic = tournament.Basic,
                        PointsToWin = tournament.PointsToWin,
                        PointsToWinLastSet = tournament.PointsToWinLastSet,
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
                }, currentRound - 1));

        return childMatch;
    }
    
    public static IList<TournamentMatch> PopulateEmptyBrackets(IList<TournamentMatch> tournamentMatches, IList<GameTeam> teams)
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

    public static IEnumerable<TournamentMatch> MoveMatchTeamDown(ICollection<TournamentMatch> tournamentMatches, TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.Game.FirstTeam != null && tournamentMatch.Game.SecondTeam != null)
        {
            throw new InvalidOperationException("Cannot move down a team if it has an opponent");
        }

        if (tournamentMatch.Game.FirstTeam == null && tournamentMatch.FirstParent != null && HasDirectAncestor(tournamentMatch.FirstParent) || 
            tournamentMatch.Game.SecondTeam == null && tournamentMatch.SecondParent != null && HasDirectAncestor(tournamentMatch.SecondParent))
        {
            throw new InvalidOperationException("Cannot move down a team that will have an opponent");
        }

        if (tournamentMatch.Game.SecondTeam == null)
        {
            var childMatchOf = FindChildMatchOf(tournamentMatches, tournamentMatch);
            if (!childMatchOf.Item2)
            {
                if (childMatchOf.Item1.Game.FirstTeam?.Title == tournamentMatch.Game.FirstTeam?.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.FirstTeam = tournamentMatch.Game.FirstTeam?.Copy();
            }
            else
            {
                if (childMatchOf.Item1.Game.SecondTeam?.Title == tournamentMatch.Game.FirstTeam?.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.SecondTeam = tournamentMatch.Game.FirstTeam?.Copy();
            }

            childMatchOf.Item1.Game = CheckIfGameIsReady(childMatchOf.Item1);

            yield return childMatchOf.Item1;
        }
        else if (tournamentMatch.Game.FirstTeam == null)
        {
            var childMatchOf = FindChildMatchOf(tournamentMatches, tournamentMatch);
            if (!childMatchOf.Item2)
            {
                if (childMatchOf.Item1.Game.FirstTeam?.Title == tournamentMatch.Game.SecondTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.FirstTeam = tournamentMatch.Game.SecondTeam.Copy();
            }
            else
            {
                if (childMatchOf.Item1.Game.SecondTeam?.Title == tournamentMatch.Game.SecondTeam.Title)
                {
                    throw new InvalidOperationException("The team was already moved down");
                }
                childMatchOf.Item1.Game.SecondTeam = tournamentMatch.Game.SecondTeam.Copy();
            }

            childMatchOf.Item1.Game = CheckIfGameIsReady(childMatchOf.Item1);

            yield return childMatchOf.Item1;
        }
    }

    private static bool HasDirectAncestor(TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.FirstParent == null && tournamentMatch.SecondParent == null)
        {
            return (tournamentMatch.Game.FirstTeam != null && tournamentMatch.Game.SecondTeam != null);
        }
        
        return (HasDirectAncestor(tournamentMatch.FirstParent) || HasDirectAncestor(tournamentMatch.SecondParent));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tournament"></param>
    /// <param name="tournamentMatch"></param>
    /// <returns>Tournament to update and tournament matches to update</returns>
    public static (Tournament, ICollection<TournamentMatch>) MatchesToUpdateInTournamentAfterWonMatch(Tournament tournament, TournamentMatch tournamentMatch)
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
            var childMatch = FindChildMatchOf(tournament.Matches, tournamentMatch);
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
            if (tournamentMatch.Round == lastRound - 1 && tournament.SingleThirdPlace && tournament.AcceptedTeams.Count >= 4)
            {
                var thirdPlaceMatch = tournament.Matches.FirstOrDefault(x => x.ThirdPlace);
                var thirdPlaceGame = thirdPlaceMatch.Game;
                if (thirdPlaceGame.FirstTeam == null)
                {
                    try
                    {
                        thirdPlaceGame.FirstTeam = GameUtils.FindOtherTeam(tournamentMatch.Game, tournamentMatch.Game.Winner).Copy();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else if (thirdPlaceGame.SecondTeam == null)
                {
                    try
                    {
                        thirdPlaceGame.SecondTeam = GameUtils.FindOtherTeam(tournamentMatch.Game, tournamentMatch.Game.Winner).Copy();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                thirdPlaceMatch.Game = CheckIfGameIsReady(thirdPlaceMatch);
                matchesToUpdate.Add(thirdPlaceMatch);
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
    private static (TournamentMatch, bool) FindChildMatchOf(ICollection<TournamentMatch> tournamentMatches, TournamentMatch parentMatch)
    {
        var child = tournamentMatches.FirstOrDefault(x => x.FirstParent?.Id == parentMatch.Id);
        if (child != null)
        {
            return (child, false);
        }

        child = tournamentMatches.FirstOrDefault(x => x.SecondParent?.Id == parentMatch.Id);
        if (child != null)
        {
            return (child, true);
        }
        
        return (null, false);
    }

    private static Game CheckIfGameIsReady(TournamentMatch tournamentMatch)
    {
        if (tournamentMatch.Game.FirstTeam == null || tournamentMatch.Game.SecondTeam == null)
        {
            return tournamentMatch.Game;
        }

        tournamentMatch.Game.Status = GameStatus.Ready;
        tournamentMatch.Game.Title = tournamentMatch.ThirdPlace ? "Third place match" : tournamentMatch.Game.FirstTeam.Title + " vs " + tournamentMatch.Game.SecondTeam.Title;
        tournamentMatch.Game.Description = "Tournament " + tournamentMatch.Tournament.Title + " match in round " + tournamentMatch.Round +
                                           " between " + tournamentMatch.Game.FirstTeam.Title + " vs " + tournamentMatch.Game.SecondTeam.Title;
        return tournamentMatch.Game;
    }
}