import api from "./api";

export const getTeams = async (signal?: AbortSignal) => {
  const res = await api.get("/Teams", { signal: signal });
  return res.data;
};

export const getUserTeams = async (userId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Users/${userId}/Teams`, { signal: signal });
  return res.data;
};

export const getTeam = async (teamId: string, signal?: AbortSignal) => {
  const res = await api.get(`/Teams/${teamId}`, { signal: signal });
  return res.data;
};

export const addTeam = async (
  teamTitle: string,
  teamPicture: string,
  teamDescription: string
) => {
  const res = await api.post("/Teams", {
    Title: teamTitle,
    PictureUrl: teamPicture,
    Description: teamDescription,
  });
  return res.data;
};

export const editTeam = async (
  teamId: string,
  teamTitle: string,
  teamPicture: string,
  teamDescription: string
) => {
  const res = await api.patch(`/Teams/${teamId}`, {
    Title: teamTitle,
    PictureUrl: teamPicture,
    Description: teamDescription,
  });
  return res.data;
};

export const deleteTeam = async (teamId: string) => {
  const res = await api.delete(`/Teams/${teamId}`);
  return res.data;
};

export const addPlayerToTeam = async (teamId: string, playerName: string) => {
  const res = await api.patch(`/Teams/${teamId}/Players`, {
    Name: playerName,
  });
  return res.data;
};

export const removePlayerFromTeam = async (
  teamId: string,
  playerId: string
) => {
  const res = await api.delete(`/Teams/${teamId}/Players/${playerId}`);
  return res.data;
};
