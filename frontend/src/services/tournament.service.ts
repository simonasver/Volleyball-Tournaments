import { TournamentType } from "../utils/types";
import api from "./api";

export const getTournaments = async (signal?: AbortSignal) => {
  const res = await api.get("/Tournaments", { signal: signal });
  return res.data;
};

export const getUserTournaments = async (userId: string, signal?: AbortSignal) => {
    const res = await api.get(`/Users/${userId}/Tournaments`, { signal: signal });
    return res.data;
};

export const getTournament = async (tournamentId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Tournaments/${tournamentId}`, { signal: signal });
  return res.data;
};

export const addTournament = async (
  title: string,
  pictureUrl: string,
  description: string,
  type: TournamentType,
  maxTeams: number,
  pointsToWin: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.post("/Tournaments", {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    Type: type,
    MaxTeams: maxTeams,
    PointsToWin: pointsToWin,
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
  type: TournamentType,
  maxTeams: number,
  pointsToWin: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.patch(`/Tournaments/${tournamentId}`, {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    Type: type,
    MaxTeams: maxTeams,
    PointsToWin: pointsToWin,
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

  export const getTournamentMatches = async (tournamentId: string, signal?: AbortSignal) => {
    const res = await api.get(`/Tournaments/${tournamentId}/Matches`, { signal: signal });
    return res.data;
  };

  export const moveTeamDown = async (tournamentId: string, matchId: string) => {
    const res = await api.patch(`/Tournaments/${tournamentId}/Matches/${matchId}/Brackets`);
    return res.data;
  }
