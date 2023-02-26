import { createSlice } from "@reduxjs/toolkit";

interface AlertState {
  type: "error" | "info" | "success" | "warning" | undefined;
  message: string;
}

const initialState: AlertState = {
  type: undefined,
  message: "",
};

const alertSlice = createSlice({
  name: "alert",
  initialState: initialState,
  reducers: {
    changeAlert(state, action) {
      const newType = action.payload.type;
      const newMessage = action.payload.message;
      state.type = newType;
      state.message = newMessage;
    },
  },
});

export const alertActions = alertSlice.actions;
export default alertSlice;
