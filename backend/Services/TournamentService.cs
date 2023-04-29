using Backend.Data.Dtos.Tournament;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Data.Entities.Utils;
using Backend.Helpers.Extensions;
using Backend.Helpers.Utils;
using Backend.Interfaces.Repositories;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentMatchRepository _tournamentMatchRepository;
    private readonly IGameTeamRepository _gameTeamRepository;

    public TournamentService(ITournamentRepository tournamentRepository, ITournamentMatchRepository tournamentMatchRepository, IGameTeamRepository gameTeamRepository)
    {
        _tournamentRepository = tournamentRepository;
        _tournamentMatchRepository = tournamentMatchRepository;
        _gameTeamRepository = gameTeamRepository;
    }
    
    public async Task<ServiceResult<IEnumerable<Tournament>>> GetAllAsync(bool all, SearchParameters searchParameters)
    {
        try
        {
            var tournaments = await _tournamentRepository.GetAllAsync(all, searchParameters);
            return ServiceResult<IEnumerable<Tournament>>.Success(tournaments);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Tournament>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<IEnumerable<Tournament>>> GetUserTournamentsAsync(SearchParameters searchParameters, string userId)
    {
        try
        {
            var userTournaments = await _tournamentRepository.GetAllUserAsync(searchParameters, userId);
            return ServiceResult<IEnumerable<Tournament>>.Success(userTournaments);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<Tournament>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<Tournament>> GetAsync(Guid tournamentId)
    {
        Tournament tournament;
        try
        {
            tournament = await _tournamentRepository.GetAsync(tournamentId);
        }
        catch (Exception ex)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }

        if (tournament == null)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status404NotFound);
        }
        
        return ServiceResult<Tournament>.Success(tournament);
    }

    public async Task<ServiceResult<IEnumerable<TournamentMatch>>> GetTournamentMatchesAsync(Guid tournamentId, bool allData)
    {
        try
        {
            var matches = await _tournamentMatchRepository.GetAllTournamentAsync(tournamentId, allData);
            return ServiceResult<IEnumerable<TournamentMatch>>.Success(matches);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<TournamentMatch>>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<Tournament>> CreateAsync(AddTournamentDto addTournamentDto, string userId)
    {
        if (addTournamentDto.MaxSets % 2 == 0)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status400BadRequest, "Max sets must be an odd number");
        }

        if (!String.IsNullOrEmpty(addTournamentDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(addTournamentDto.PictureUrl)))
            {
                return ServiceResult<Tournament>.Failure(StatusCodes.Status400BadRequest, "Provided picture url was not an image");
            }
        }

        var newTournament = new Tournament()
        {
            Title = addTournamentDto.Title,
            PictureUrl = addTournamentDto.PictureUrl,
            Description = addTournamentDto.Description,
            Basic = addTournamentDto.Basic,
            SingleThirdPlace = addTournamentDto.SingleThirdPlace,
            MaxTeams = addTournamentDto.MaxTeams,
            PointsToWin = addTournamentDto.PointsToWin,
            PointsToWinLastSet = addTournamentDto.PointsToWinLastSet,
            PointDifferenceToWin = addTournamentDto.PointDifferenceToWin,
            MaxSets = addTournamentDto.MaxSets,
            PlayersPerTeam = addTournamentDto.PlayersPerTeam,
            IsPrivate = addTournamentDto.IsPrivate,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            Status = TournamentStatus.Open,
            OwnerId = userId
        };

        try
        {
            var createdTournament = await _tournamentRepository.CreateAsync(newTournament);
            return ServiceResult<Tournament>.Success(createdTournament);
        }
        catch (Exception ex)
        {
            return ServiceResult<Tournament>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(Tournament tournament)
    {
        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(EditTournamentDto editTournamentDto, Tournament tournament)
    {
        if (tournament.Status == TournamentStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot edit finished tournament");
        }

        if (editTournamentDto.MaxSets % 2 == 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Max sets must be an odd number");
        }

        if (editTournamentDto.Title != null)
        {
            tournament.Title = editTournamentDto.Title;
        }

        if (!String.IsNullOrEmpty(editTournamentDto.PictureUrl))
        {
            if (!(await Utils.IsLinkImage(editTournamentDto.PictureUrl)))
            {
                return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Provided picture url was not an image");
            }
            tournament.PictureUrl = editTournamentDto.PictureUrl;
        }

        if (editTournamentDto.Description != null)
        {
            tournament.Description = editTournamentDto.Description;
        }

        if (editTournamentDto.SingleThirdPlace != null)
        {
            tournament.SingleThirdPlace = editTournamentDto.SingleThirdPlace ?? false;
        }
        
        if (editTournamentDto.Basic != null)
        {
            tournament.Basic = editTournamentDto.Basic ?? false;
        }

        if (editTournamentDto.MaxTeams != null)
        {
            tournament.MaxTeams = editTournamentDto.MaxTeams ?? tournament.MaxTeams;
        }

        if (editTournamentDto.PointsToWin != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PointsToWin = editTournamentDto.PointsToWin ?? tournament.PointsToWin;
            }
        }

        if (editTournamentDto.PointsToWinLastSet != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PointsToWinLastSet = editTournamentDto.PointsToWinLastSet ?? tournament.PointsToWinLastSet;
            }
        }

        if (editTournamentDto.PointDifferenceToWin != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PointDifferenceToWin = editTournamentDto.PointDifferenceToWin ?? tournament.PointDifferenceToWin;
            }
        }

        if (editTournamentDto.MaxSets != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.MaxSets = editTournamentDto.MaxSets ?? tournament.MaxSets;
            }
        }
        
        if (editTournamentDto.PlayersPerTeam != null)
        {
            if (tournament.Status < TournamentStatus.Started)
            {
                tournament.PlayersPerTeam = editTournamentDto.PlayersPerTeam ?? tournament.PlayersPerTeam;
            }
        }

        if (editTournamentDto.IsPrivate != null)
        {
            tournament.IsPrivate = editTournamentDto.IsPrivate ?? tournament.IsPrivate;
        }
        
        tournament.LastEditDate = DateTime.Now;

        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateMatchAsync(TournamentMatch match)
    {
        try
        {
            await _tournamentMatchRepository.UpdateAsync(match);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Tournament tournament)
    {
        try
        {
            await _tournamentRepository.DeleteAsync(tournament.Id);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> TeamRequestJoinAsync(Tournament tournament, Team team)
    {
        if (tournament.RequestedTeams.Any(x => x.Id == team.Id) || tournament.AcceptedTeams.Any(x => x.Title == team.Title))
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team already requested to join this game");
        }

        if (team.Players.Count != tournament.PlayersPerTeam && tournament.PlayersPerTeam != 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Teams in this tournament are required to have " + tournament.PlayersPerTeam + " players");
        }

        if (team.Players.Count == 0)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team must have at least 1 player");
        }

        tournament.RequestedTeams.Add(team);

        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> AddTeamAsync(AddTeamToTournamentDto addTeamToTournamentDto,
        Tournament tournament)
    {
        if (tournament.AcceptedTeams.Count >= tournament.MaxTeams)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Tournament is already full");
        }

        if (!tournament.RequestedTeams.Any(x => x.Id == addTeamToTournamentDto.TeamId))
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team has not requested to join the game");
        }

        var team = tournament.RequestedTeams.FirstOrDefault(x => x.Id == addTeamToTournamentDto.TeamId);
        
        if ((team.Players.Count != tournament.PlayersPerTeam && tournament.PlayersPerTeam != 0) || team.Players.Count == 0)
        {
            tournament.RequestedTeams.Remove(team);
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team has 0 or another incompatible number of players for this game");
        }

        tournament = TournamentUtils.AddTeamToTournament(tournament, team);
        
        tournament.LastEditDate = DateTime.Now;

        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> RemoveTeamAsync(Tournament tournament, Guid teamId)
    {
        if (tournament.Status >= TournamentStatus.Started)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Game is already started");
        }

        var team = tournament.AcceptedTeams.FirstOrDefault(x => x.Id == teamId);

        if (team != null)
        {
            tournament.AcceptedTeams.Remove(team);
        }

        try
        {
            await _gameTeamRepository.DeleteAsync(team.Id);
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        tournament.LastEditDate = DateTime.Now;

        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> StartAsync(Tournament tournament)
    {
        if (tournament.Status == TournamentStatus.Started)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Tournament is already started");
        }

        if (tournament.Status == TournamentStatus.Finished)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, ("Tournament is already finished"));
        }

        if (tournament.AcceptedTeams.Count < 2)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Tournament does not have at least two teams");
        }

        tournament.Status = TournamentStatus.Started;

        var roundCount = tournament.AcceptedTeams.CountTournamentRounds();
        tournament.FinalRound = roundCount;
        var generatedMatches = TournamentUtils.GenerateEmptyBracket(tournament, roundCount).ToList();

        var populatedMatches = TournamentUtils.PopulateEmptyBrackets(generatedMatches, tournament.AcceptedTeams);

        tournament.LastEditDate = DateTime.Now;

        tournament.Matches = populatedMatches;

        try
        {
            await _tournamentRepository.UpdateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    public async Task<ServiceResult<bool>> MoveBracketAsync(Tournament tournament, Guid matchId)
    {
        var tournamentMatchesResult = await GetTournamentMatchesAsync(tournament.Id, true);

        if (!tournamentMatchesResult.IsSuccess)
        {
            return ServiceResult<bool>.Failure(tournamentMatchesResult.ErrorStatus, tournamentMatchesResult.ErrorMessage);
        }

        var tournamentMatches = tournamentMatchesResult.Data;

        var match = tournamentMatches.FirstOrDefault(x => x.Id == matchId);

        if (match == null)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status404NotFound);
        }

        if (match.Round == tournament.FinalRound)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Cannot move down from the final round");
        }
        
        try
        {
            var matchesToUpdate = TournamentUtils.MoveMatchTeamDown(tournamentMatches.ToList(), match);
            foreach (var tournamentMatch in matchesToUpdate)
            {
                try
                {
                    await _tournamentMatchRepository.UpdateAsync(tournamentMatch);
                }
                catch (Exception ex)
                {
                    return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, ex.Message);
        }
        
        return ServiceResult<bool>.Success();
    }

    public async Task<ServiceResult<bool>> GenerateAsync(int teamAmount, string userId)
    {
        if (teamAmount == null || teamAmount < 0 || teamAmount > 128)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status400BadRequest, "Team amount must be between 1 and 128");
        }
        
        var generatedNamePrefix = DateTime.Now.ToShortTimeString();

        var tournament = new Tournament()
        {
            Title = "Tournament " + generatedNamePrefix,
            Description = "Tournament is generated for testing purposes only",
            SingleThirdPlace = false,
            Basic = true,
            MaxTeams = 128,
            IsPrivate = false,
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            Status = TournamentStatus.Open,
            RequestedTeams = new List<Team>(),
            AcceptedTeams = new List<GameTeam>(),
            PointsToWin = 1,
            PointsToWinLastSet = 1,
            PointDifferenceToWin = 0,
            MaxSets = 1,
            PlayersPerTeam = 0,
            OwnerId = userId
        };

        for (int i = 0; i < teamAmount; i++)
        {
            var team = new GameTeam()
            {
                Title = "Team " + generatedNamePrefix + " " + (i+1),
                Description = "Team is generated for testing purposes only",
                Players = new List<GameTeamPlayer>(),
            };
            for (int j = 0; j < 6; j++)
            {
                var player = new GameTeamPlayer()
                {
                    Name = "Player " + (j+1)
                };
                team.Players.Add(player);
            }
            tournament.AcceptedTeams.Add(team);
        }

        try
        {
            await _tournamentRepository.CreateAsync(tournament);
            return ServiceResult<bool>.Success();
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}