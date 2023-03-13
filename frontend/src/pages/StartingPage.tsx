import { Grid, Typography } from "@mui/material";
import Alert from "@mui/material/Alert";
import React from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../components/layout/Layout";
import StartingButton from "../components/starting/StartingButton";
import StartingButtonRow from "../components/starting/StartingButtonRow";
import { useAppDispatch, useAppSelector } from "../hooks";
import { alertActions } from "../store/alert-slice";

const StartingPage = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  
  const alertData = useAppSelector((state) => state.alert);

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const clearAlertTimeout = setTimeout(() => {
      dispatch(alertActions.clearAlert());
    }, 10000);
    return () => {
      clearTimeout(clearAlertTimeout);
      dispatch(alertActions.clearAlert());
    };
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
        <Grid item>
          {alertData.type && (
            <Alert severity={alertData.type}>{alertData.message}</Alert>
          )}
          <br />
        </Grid>
        <Grid item textAlign="center">
          <Typography variant="h3">Volleyball tournaments!</Typography>
          <br />
          <Typography variant="subtitle1">
            You can host your volleyball tournaments here!
          </Typography>
          <br />
          <br />
        </Grid>
        <Grid item>
          <Typography variant="h6">Useful links:</Typography>
          <br />
        </Grid>
        <Grid item>
          {user && (
            <StartingButtonRow>
              <StartingButton title="My teams" onClick={() => { navigate("/myteams"); }}/>
              <StartingButton title="My tournaments" />
              <StartingButton title="My games" />
            </StartingButtonRow>
          )}
          <StartingButtonRow>
            <StartingButton title="Public tournaments" />
            <StartingButton title="Public games" />
          </StartingButtonRow>
        </Grid>
      </Grid>
    </Layout>
  );
};

export default StartingPage;
