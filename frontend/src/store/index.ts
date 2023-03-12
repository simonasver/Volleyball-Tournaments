import { configureStore } from "@reduxjs/toolkit";
import alertSlice from "./alert-slice";

import authSlice, { IUser } from "./auth-slice";

function getTokensFromLocalStorage() {
  const persistedTokensJSON = localStorage.getItem("tokens");
  if (!persistedTokensJSON) return undefined;
  try {
    return JSON.parse(persistedTokensJSON);
  }
  catch(e) {
    localStorage.removeItem('tokens');
    return undefined;
  }
}

function getUserObjectFromLocalStorage(): IUser | undefined {
  const persistedUserJSON = localStorage.getItem("user");
  if (!persistedUserJSON) return undefined;
  try {
    return JSON.parse(persistedUserJSON);
  }
  catch(e) {
    localStorage.removeItem('user');
    return undefined;
  }
}

const persistedTokens: { accessToken: string, refreshToken: string } | undefined = getTokensFromLocalStorage();
const persistedUser: IUser | undefined = getUserObjectFromLocalStorage();

const store = configureStore({
  reducer: { alert: alertSlice.reducer, auth: authSlice.reducer },
  preloadedState: { alert: undefined, auth: { tokens: persistedTokens, user: persistedUser } },
});

store.subscribe(() => {
  const { auth } = store.getState();
  if (auth) {
    localStorage.setItem("tokens", JSON.stringify(auth.tokens));
    localStorage.setItem("user", JSON.stringify(auth.user));
  } else {
    localStorage.removeItem("tokens");
    localStorage.removeItem("user");
  }
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
export default store;
