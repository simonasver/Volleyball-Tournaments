import { AppDispatch } from "../store";
import { authActions } from "../store/auth-slice";
import api from "./api";

export const login = async (username: string, password: string) => {
  const res = await api.post("/Tokens", {
    UserName: username,
    Password: password,
  });
  return res.data;
};

export const register = async (
  username: string,
  fullname: string,
  email: string,
  password: string
) => {
  const res = await api.post("/Users", {
    UserName: username,
    FullName: fullname,
    Email: email,
    Password: password,
  });
  return res.data;
};

export const refresh = async (token: string) => {
  const res = await api.put("/Tokens", {
    token,
  });
  return res.data;
};

export const logout = async (dispatch: AppDispatch, signal?: AbortSignal) => {
  try {
    const res = await api.delete("/Tokens", { signal: signal });
    return res.data;
  } catch (e) {
    return Promise.reject(e);
  } finally {
    dispatch(authActions.clearUser());
  }
};
