import axios from "axios";
import { User } from "../store/auth-slice";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function errorMessageFromAxiosError(e: any): string {
  if (!axios.isCancel(e)) {
    if (e.response) {
      return e.response.data || "Error";
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
  return user.roles?.includes("admin") ?? false;
}
