import { User } from "../store/auth-slice";
import { formatPaginationDataToQuery } from "../utils/helpers";
import { PageData } from "../utils/types";
import api from "./api";

export const getUsers = async (pageNumber: number, pageSize: number, signal?: AbortSignal): Promise<{ data: User[], pagination: PageData }> => {
  const res = await api.get(`/Users?${formatPaginationDataToQuery({ pageNumber, pageSize })}`, { signal: signal });
  return res.data;
};

export const generateTournament = async (teamNumber: number) => {
  const res = await api.post(`/Tournaments/generate?teamAmount=${teamNumber}`);
  return res.data;
};

export const ban = async (userId: string, ban: boolean) => {
  const res = await api.patch(`/Users/${userId}/Banned`, { Ban: ban });
  return res.data;
}