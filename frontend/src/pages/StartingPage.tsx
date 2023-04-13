import React from "react";
import { Button, Grid, Typography } from "@mui/material";
import { Stack } from "@mui/system";
import { useNavigate } from "react-router-dom";
import Layout from "../components/layout/Layout";
import { useAppSelector } from "../utils/hooks";

const StartingPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  return (
    <Layout header>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <Grid item textAlign="center">
          <br />
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
            onClick={() => navigate("/tournaments")}
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
            onClick={() => navigate("/games")}
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
