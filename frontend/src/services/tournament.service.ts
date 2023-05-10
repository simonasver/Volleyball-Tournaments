import { formatPaginationDataToQuery, formatSearchInputDataToQuery } from "../utils/helpers";
import { GameTeam, PageData, Tournament, TournamentMatch } from "../utils/types";
import api from "./api";

export const getTournaments = async (pageNumber: number, pageSize: number, searchInput: string, signal?: AbortSignal): Promise<{ data: Tournament[], pagination: PageData }> => {
  const res = await api.get(`/Tournaments?${formatPaginationDataToQuery({ pageNumber, pageSize })}${formatSearchInputDataToQuery(searchInput)}`, { signal: signal });
  return res.data;
};

export const getUserTournaments = async (userId: string, pageNumber: number, pageSize: number, searchInput: string, signal?: AbortSignal): Promise<{ data: Tournament[], pagination: PageData }> => {
    const res = await api.get(`/Users/${userId}/Tournaments?${formatPaginationDataToQuery({ pageNumber, pageSize })}${formatSearchInputDataToQuery(searchInput)}`, { signal: signal });
    return res.data;
};

export const getTournament = async (tournamentId: string, signal?: AbortSignal): Promise<Tournament> => {
  const res = await api.get(`/Tournaments/${tournamentId}`, { signal: signal });
  return res.data;
};

export const addTournament = async (
  title: string,
  pictureUrl: string,
  description: string,
  basic: boolean,
  singleThirdPlace: boolean,
  maxTeams: number,
  pointsToWin: number,
  pointsToWinLastSet: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.post("/Tournaments", {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    Basic: basic,
    SingleThirdPlace: singleThirdPlace,
    MaxTeams: maxTeams,
    PointsToWin: pointsToWin,
    PointsToWinLastSet: pointsToWinLastSet,
    PointDifferenceToWin: pointDifferenceToWin,
    MaxSets: maxSets,
    PlayersPerTeam: playersPerTeam,
    IsPrivate: isPrivate,
  });
  return res.data;
};

export const editTournament = async (
  tournamentId: string,
  title: string,
  pictureUrl: string,
  description: string,
  singleThirdPlace: boolean,
  basic: boolean,
  maxTeams: number,
  pointsToWin: number,
  pointsToWinLastSet: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.patch(`/Tournaments/${tournamentId}`, {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    SingleThirdPlace: singleThirdPlace,
    Basic: basic,
    MaxTeams: maxTeams,
    PointsToWin: pointsToWin,
    pointsToWinLastSet: pointsToWinLastSet,
    PointDifferenceToWin: pointDifferenceToWin,
    MaxSets: maxSets,
    PlayersPerTeam: playersPerTeam,
    IsPrivate: isPrivate,
  });
  return res.data;
};

export const deleteTournament = async (tournamentId: string) => {
    const res = await api.delete(`/Tournaments/${tournamentId}`);
    return res.data;
};

export const joinTournament = async (tournamentId: string, teamId: string) => {
    const res = await api.post(`/Tournaments/${tournamentId}/RequestedTeams`, {
      TeamId: teamId,
    });
    return res.data;
  };
  
  export const addTeamToTournament = async (tournamentId: string, teamId: string) => {
    const res = await api.post(`/Tournaments/${tournamentId}/AcceptedTeams`, {
      TeamId: teamId,
    });
    return res.data;
  };
  
  export const removeTeamFromTournament = async (tournamentId: string, team: string) => {
    const res = await api.delete(`/Tournaments/${tournamentId}/AcceptedTeams?teamId=${team}`);
    return res.data;
  };
  
  export const startTournament = async (tournamentId: string) => {
    const res = await api.patch(`/Tournaments/${tournamentId}/Status`);
    return res.data;
  };

  export const getTournamentMatches = async (tournamentId: string, signal?: AbortSignal): Promise<TournamentMatch[]> => {
    const res = await api.get(`/Tournaments/${tournamentId}/Matches`, { signal: signal });
    return res.data;
  };

  export const moveTeamDown = async (tournamentId: string, matchId: string) => {
    const res = await api.patch(`/Tournaments/${tournamentId}/Matches/${matchId}/Brackets`);
    return res.data;
  };

  export const generateTournament = async (teamNumber: number) => {
    const res = await api.post(`/Tournaments/generate?teamAmount=${teamNumber}`);
    return res.data;
  };

  export const addTournamentManager = async (tournamenId: string, playerId: string) => {
    const res = await api.patch(`/Tournaments/${tournamenId}/Managers`, {
      ManagerId: playerId
    });
    return res.data;
  };
  
  export const removeTournamentManager = async (tournamentId: string, playerId: string) => {
    const res = await api.delete(`/Tournaments/${tournamentId}/Managers/${playerId}`);
    return res.data;
  };

  export const reorderTeams = async (tournamentId: string, numbers: { [id: string]: number }) => {
    console.log(numbers);
    const res = await api.patch(`/Tournaments/${tournamentId}/AcceptedTeams/Order`, {
      UpdatedNumbers: numbers
    });
    return res.data;
  };
