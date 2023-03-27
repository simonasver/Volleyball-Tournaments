import api from "./api";

export const getGames = async (signal?: AbortSignal) => {
  const res = await api.get("/Game", { signal: signal });
  return res.data;
};

export const getUserGames = async (userId: string, signal?: AbortSignal) => {
  const res = await api.get(`/User/${userId}/Game`, { signal: signal });
  return res.data;
};

export const getGame = async (gameId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Game/${gameId}`, { signal: signal });
  return res.data;
};

export const addGame = async (
  title: string,
  description: string,
  pointsToWin: number,
  pointDifferenceToWin: number,
  maxSets: number,
  playersPerTeam: number,
  isPrivate: boolean
) => {
  const res = await api.post("/Game", {
    Title: title,
    Description: description,
    PointsToWin: pointsToWin,
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
  description?: string,
  pointsToWin?: number,
  pointDifferenceToWin?: number,
  maxSets?: number,
  playersPerTeam?: number,
  isPrivate?: boolean
) => {
  const res = await api.put(`/Game/${gameId}`, {
    Title: title,
    Description: description,
    PointsToWin: pointsToWin,
    PointDifferenceToWin: pointDifferenceToWin,
    MaxSets: maxSets,
    PlayersPerTeam: playersPerTeam,
    IsPrivate: isPrivate,
  });
  return res.data;
};
