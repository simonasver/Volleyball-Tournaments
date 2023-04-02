import api from "./api";

export const getGameSets = async (gameId: string, signal?: AbortSignal) => {
    const res = await api.get(`/Games/${gameId}/Sets`, { signal: signal });
    return res.data;
  };