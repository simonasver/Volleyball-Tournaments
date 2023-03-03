import { createSlice } from "@reduxjs/toolkit";

const initialState = {
  user: null,
};

const authSlice = createSlice({
  name: "auth",
  initialState: initialState,
  reducers: {
    changeUser(state, action) {
      const newUser = action.payload.user;
      state.user = newUser;
    },
  },
});

export const authActions = authSlice.actions;
export default authSlice;
