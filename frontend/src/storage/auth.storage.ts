import { refresh } from "../services/auth.service";
import { AppDispatch } from "../store";
import { authActions } from "../store/auth-slice";

interface IUser {
  userName: string;
  email: string;
  roles: string[];
}

export const getUserData = () => {
  const userData = localStorage.getItem("user");
  if (userData) {
    return JSON.parse(userData);
  }
  return null;
};

export const refreshUserData = async (
  refreshToken: string,
  dispatch: AppDispatch
) => {
  const newAccessToken = await refresh(refreshToken);
  if (newAccessToken) {
    setUserData(dispatch, newAccessToken, refreshToken);
  } else {
    setUserData(dispatch, undefined, undefined, undefined);
  }
};

export const setUserData = (
  dispatch: AppDispatch,
  accessToken?: string,
  refreshToken?: string,
  userData?: IUser
) => {
  if (!accessToken && !refreshToken) {
    localStorage.removeItem("user");
    dispatch(
      authActions.changeUser({
        user: null,
      })
    );
    return;
  }
  let newUserData;
  if (!userData) {
    newUserData = getUserData();
  } else {
    newUserData = userData;
  }
  const user = {
    ...newUserData,
    accessToken: accessToken,
    refreshToken: refreshToken,
  };
  localStorage.setItem("user", JSON.stringify(user));
  dispatch(
    authActions.changeUser({
      user: user,
    })
  );
};

export const updateUserAccessToken = (accessToken: string) => {
  const user = JSON.parse(localStorage.getItem("user") ?? "");
  user.accessToken = accessToken;
  localStorage.setItem("user", JSON.stringify(user));
};
