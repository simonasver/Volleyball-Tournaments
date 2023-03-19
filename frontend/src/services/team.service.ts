import api from "./api";

export const getTeams = async () => {
  const res = await api.get("/Team");
  return res.data;
};

export const getUserTeams = async (userId: string) => {
  const res = await api.get(`/User/${userId}/Team`);
  return res.data;
};

export const getTeam = async (teamId: string) => {
  const res = await api.get(`/Team/${teamId}`);
  return res.data;
};

export const addTeam = async (
  teamTitle: string,
  teamPicture: string,
  teamDescription: string
) => {
  const res = await api.post("/Team", {
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
  const res = await api.put(`/Team/${teamId}`, {
    Title: teamTitle,
    PictureUrl: teamPicture,
    Description: teamDescription,
  });
  return res.data;
};

export const deleteTeam = async (teamId: string) => {
  const res = await api.delete(`/Team/${teamId}`);
  return res.data;
};

export const addPlayerToTeam = async (teamId: string, playerName: string) => {
  const res = await api.post(`/Team/${teamId}/Player`, {
    Name: playerName,
  });
  return res.data;
};

export const removePlayerFromTeam = async (
  teamId: string,
  playerId: string
) => {
  const res = await api.delete(`/Team/${teamId}/Player/${playerId}`);
  return res.data;
};
