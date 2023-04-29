import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface Tokens {
  accessToken: string;
  refreshToken: string;
}

export interface User {
  id: string;
  userName: string;
  email: string;
  fullName?: string;
  profilePictureUrl?: string;
  registerDate?: string;
  lastLoginDate?: string;
  roles: Roles[];
  banned?: boolean;
}

export enum Roles {
  User = "User",
  Admin = "Admin"
}

interface AuthState {
  tokens: Tokens | undefined;
  user: User | undefined;
}

const initialState: AuthState = {
  tokens: undefined,
  user: undefined,
};

const authSlice = createSlice({
  name: "auth",
  initialState: initialState,
  reducers: {
    changeTokens(state, action: PayloadAction<Tokens>) {
      state.tokens = action.payload;
    },
    changeUser(state, action: PayloadAction<User>) {
      state.user = action.payload;
    },
    clearUser(state) {
      state.user = undefined;
    },
  },
});

export const authActions = authSlice.actions;
export default authSlice;
