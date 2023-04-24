import { GameScore } from "../utils/types";
import api from "./api";

export const getGames = async (signal?: AbortSignal) => {
  const res = await api.get("/Games", { signal: signal });
  return res.data;
};

export const getUserGames = async (userId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Users/${userId}/Games`, { signal: signal });
  return res.data;
};

export const getGame = async (gameId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Games/${gameId}`, { signal: signal });
  return res.data;
};

export const addGame = async (
  title: string,
  pictureUrl: string,
  description: string,
  basic: boolean,
  pointsToWin: number,
  pointsToWinLastSet: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.post("/Games", {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    Basic: basic,
    PointsToWin: pointsToWin,
    pointsToWinLastSet: pointsToWinLastSet,
    PointDifferenceToWin: pointDifferenceToWin,
    MaxSets: maxSets,
    PlayersPerTeam: playersPerTeam,
    IsPrivate: isPrivate,
  });
  return res.data;
};

export const editGame = async (
  gameId: string,
  title?: string,
  pictureUrl?: string,
  description?: string,
  basic?: boolean,
  pointsToWin?: number,
  pointsToWinLastSet?: number,
  pointDifferenceToWin?: number,
  maxSets?: number,
  playersPerTeam?: number,
  isPrivate?: boolean
) => {
  const res = await api.patch(`/Games/${gameId}`, {
    Title: title,
    PictureUrl: pictureUrl,
    Description: description,
    Basic: basic,
    PointsToWin: pointsToWin,
    pointsToWinLastSet: pointsToWinLastSet,
    PointDifferenceToWin: pointDifferenceToWin,
    MaxSets: maxSets,
    PlayersPerTeam: playersPerTeam,
    IsPrivate: isPrivate,
  });
  return res.data;
};

export const deleteGame = async (gameId: string) => {
  const res = await api.delete(`/Games/${gameId}`);
  return res.data;
};

export const joinGame = async (gameId: string, teamId: string) => {
  const res = await api.post(`/Games/${gameId}/RequestedTeams`, {
    TeamId: teamId,
  });
  return res.data;
};

export const addTeamToGame = async (gameId: string, teamId: string) => {
  const res = await api.post(`/Games/${gameId}/GameTeams`, {
    TeamId: teamId,
  });
  return res.data;
};

export const removeTeamFromGame = async (gameId: string, team: boolean) => {
  const res = await api.delete(`/Games/${gameId}/GameTeams?team=${team}`);
  return res.data;
};

export const startGame = async (gameId: string) => {
  const res = await api.patch(`/Games/${gameId}/Status`);
  return res.data;
};

export const getGameSets = async (gameId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Games/${gameId}/Sets`, { signal: signal });
  return res.data;
};

export const changeGameSetScore = async (
  gameId: string,
  setId: string,
  playerId: string,
  change: boolean
) => {
  const res = await api.patch(
    `/Games/${gameId}/Sets/${setId}/Players/${playerId}/Score`,
    {
      Change: change,
    }
  );
  return res.data;
};

export const changeGameSetStats = async (gameId: string, setId: string, playerId: string, type: GameScore, change: boolean) => {
  const res = await api.patch(`/Games/${gameId}/Sets/${setId}/Players/${playerId}/Stats`, 
  {
    Type: type-1,
    Change: change
  });
  return res.data;
}

export const getGameLogs = async (gameId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Games/${gameId}/Logs`, { signal: signal });
  return res.data;
};
