import { Grid, Typography } from "@mui/material";
import Alert from "@mui/material/Alert";
import React from "react";
import Layout from "../components/layout/Layout";
import { useAppDispatch, useAppSelector } from "../hooks";
import { alertActions } from "../store/alert-slice";

const StartingPage = () => {
  const dispatch = useAppDispatch();
  const alertData = useAppSelector((state) => state.alert);

  React.useEffect(() => {
    setTimeout(() => {
      dispatch(alertActions.changeAlert({ type: undefined, message: "" }));
    }, 10000);
  }, []);

  return (
    <Layout header>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        {alertData.type && (
          <Alert severity={alertData.type}>{alertData.message}</Alert>
        )}
        <br />
        <Typography variant="h3">Welcome to our page!</Typography>
      </Grid>
    </Layout>
  );
};

export default StartingPage;
