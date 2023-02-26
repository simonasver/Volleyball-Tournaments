import { setUserData } from "../storage/auth.storage";
import { AppDispatch } from "../store";
import api from "./api";

export const login = async (username: string, password: string) => {
  const res = await api.post("/Auth/login", {
    UserName: username,
    Password: password,
  });
  return res.data;
};

export const register = async (
  email: string,
  username: string,
  password: string
) => {
  const res = await api.post("/users", {
    email,
    username,
    password,
  });
  return res.data;
};

export const refresh = async (token: string) => {
  const res = await api.put("/tokens", {
    token,
  });
  return res.data;
};

export const logout = async (
  token: string,
  userId: string,
  dispatch: AppDispatch
) => {
  try {
    const res = await api.delete(`/tokens/${token}/users/${userId}`);
    return res.data;
  } catch (e) {
    return Promise.reject(e);
  } finally {
    setUserData(dispatch, undefined, undefined, undefined);
  }
};

export const getUserId = async (userName: string) => {
  const res = await api.get(`/users/${userName}/userIds`);
  return res.data;
};
