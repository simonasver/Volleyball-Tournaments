import { User, UserRole } from "../store/auth-slice";
import { formatPaginationDataToQuery } from "../utils/helpers";
import { PageData } from "../utils/types";
import api from "./api";

export const getUser = async (id: string, signal?: AbortSignal) => {
  const res = await api.get("/Users/" + id, { signal: signal });
  return res.data;
};

export const editUser = async (
  id: string,
  profilePictureUrl = "",
  fullName = "",
  signal?: AbortSignal
) => {
  const res = await api.patch(
    "/Users/" + id,
    {
      ProfilePictureUrl: profilePictureUrl,
      FullName: fullName,
    },
    { signal: signal }
  );
  return res.data;
};

export const getUsers = async (pageNumber: number, pageSize: number, signal?: AbortSignal): Promise<{ data: User[], pagination: PageData }> => {
  const res = await api.get(`/Users?${formatPaginationDataToQuery({ pageNumber, pageSize })}`, { signal: signal });
  return res.data;
};

export const ban = async (userId: string, ban: boolean) => {
  const res = await api.patch(`/Users/${userId}/Banned`, { Ban: ban });
  return res.data;
};

export const addRemoveRole = async (userId: string, role: UserRole, action: boolean) => {
  const res = await api.patch(`/Users/${userId}/Roles`, { Role: role, Add: action });
  return res.data;
};
