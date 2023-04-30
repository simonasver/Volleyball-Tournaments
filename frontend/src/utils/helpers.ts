import axios from "axios";
import { UserRole, User } from "../store/auth-slice";
import { Game } from "./types";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function errorMessageFromAxiosError(e: any): string {
  if (!axios.isCancel(e)) {
    if(e.message === "Network Error"){
      return "Connection error";
    }

    if (e.response) {
      if(e.response.status === 500) {
        return "Error";
      } else {
        const value = e?.response?.data?.errors ? Object.values(e?.response?.data?.errors)[0] : e?.response?.data?.title || e?.response?.data?.message || e?.response?.data || "Error";
        return value;
      }
    } else if (e.request) {
      return "Connection error";
    } else {
      return "Error";
    }
  } else {
    return "";
  }
}

export function isAdmin(user: User | undefined): boolean {
  if (user === undefined) return false;
  return user.roles?.includes(UserRole.Admin) ?? false;
}

export function isGameFull(game: Game): boolean {
  return !!game.firstTeam && !!game.secondTeam;
}

export function getDefaultPaginationData(): { pageNumber: number, pageSize: number } {
  return { pageNumber: 1, pageSize: getDefaultPageSize() };
}

export function formatPaginationDataToQuery(data: { pageNumber: number, pageSize: number }): string {
  return `pageNumber=${data.pageNumber}&pageSize=${data.pageSize}`;
}

export const getDefaultPageSize = () => 10;
