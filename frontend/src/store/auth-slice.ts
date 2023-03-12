import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ITokens {
  accessToken: string;
  refreshToken: string;
}

export interface IUser {
  id: string;
  userName: string;
  email: string;
  fullName?: string;
  profilePictureUrl?: string;
  registerDate?: string;
  lastLoginDate?: string;
}

interface AuthState {
  tokens: ITokens | undefined;
  user: IUser | undefined;
}

const initialState: AuthState = {
  tokens: undefined,
  user: undefined,
};

const authSlice = createSlice({
  name: "auth",
  initialState: initialState,
  reducers: {
    changeTokens(state, action: PayloadAction<ITokens>) {
      state.tokens = action.payload;
      console.log("new tokens");
      console.log(action.payload);
    },
    changeUser(state, action: PayloadAction<IUser>) {
      state.user = action.payload;
    },
    clearUser(state) {
      state.user = undefined;
    },
  },
});

export const authActions = authSlice.actions;
export default authSlice;
