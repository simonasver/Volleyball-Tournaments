import { Button, Grid, Typography } from "@mui/material";
import Alert from "@mui/material/Alert";
import { Stack } from "@mui/system";
import React from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../components/layout/Layout";
import { useAppDispatch, useAppSelector } from "../utils/hooks";
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
        {!user && (
          <>
            <Grid item>
              <Typography variant="h6">
                To access more features, login:
              </Typography>
              <br />
            </Grid>
            <Stack direction="row" spacing={2}>
              <Button
                variant="contained"
                sx={{
                  size: { xs: "small", md: "medium" },
                  whiteSpace: "nowrap",
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                }}
                onClick={() => navigate("/login")}
              >
                Login
              </Button>
              <Button
                variant="contained"
                sx={{
                  size: { xs: "small", md: "medium" },
                  whiteSpace: "nowrap",
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                }}
                onClick={() => navigate("/register")}
              >
                Register
              </Button>
            </Stack>
            <br />
          </>
        )}
        <Grid item>
          <Typography variant="h6">Useful links:</Typography>
          <br />
        </Grid>
        <Stack direction="row" spacing={2}>
          <Button
            variant="contained"
            sx={{
              size: { xs: "small", md: "medium" },
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
            onClick={() => undefined}
          >
            Tournaments
          </Button>
          <Button
            variant="contained"
            sx={{
              size: { xs: "small", md: "medium" },
              whiteSpace: "nowrap",
              overflow: "hidden",
              textOverflow: "ellipsis",
            }}
            onClick={() => undefined}
          >
            Games
          </Button>
          {user && (
            <Button
              variant="contained"
              sx={{
                size: { xs: "small", md: "medium" },
                whiteSpace: "nowrap",
                overflow: "hidden",
                textOverflow: "ellipsis",
              }}
              onClick={() => navigate("/myteams")}
            >
              Teams
            </Button>
          )}
        </Stack>
      </Grid>
    </Layout>
  );
};

export default StartingPage;
