import { Alert, Slide, Snackbar } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import React from "react";
import { alertActions } from "../../store/alert-slice";

const NotificationSnackbar = () => {
  const dispatch = useAppDispatch();

  const alertData = useAppSelector((state) => state.alert);

  return (
    <Snackbar
      open={!!alertData.type}
      TransitionComponent={TransitionUp}
      onClose={() => dispatch(alertActions.clearAlert())}
      anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      autoHideDuration={5000}
    >
      <Alert severity={alertData.type}>{alertData.message}</Alert>
    </Snackbar>
  );
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const TransitionUp = (props: any) => {
  return <Slide {...props} direction="up" />;
};

export default NotificationSnackbar;
