import api from "./api";

export const generateTournament = async (teamNumber: number) => {
  const res = await api.post(`/Tournaments/generate?teamAmount=${teamNumber}`);
  return res.data;
};