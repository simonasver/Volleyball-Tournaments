import React from "react";
import { Button, Grid, TextField, Typography } from "@mui/material";
import Layout from "../../components/layout/Layout";
import GameList from "../../components/game/GameList";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import BackButton from "../../components/layout/BackButton";

const MyGamesPage = () => {
  const navigate = useNavigate();

  const [searchInput, setSearchInput] = React.useState("");

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
      navigate("/", { replace: true });
    }
  }, []);

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <Grid item sx={{ width: { xs: "100%", md: "70%" } }}>
        <Typography variant="h4">My games</Typography>
        <br />
        <br />
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="space-between"
          >
            <Grid item>
              <BackButton address="/games" title="All games" />
            </Grid>
            <Grid item>
              <TextField
                size="small"
                label="Search by title"
                variant="outlined"
                onChange={(event: React.ChangeEvent<HTMLInputElement>) =>
                  setSearchInput(event.target.value)
                }
                value={searchInput}
              />
            </Grid>
            <Grid item>
              <Button
                variant="contained"
                onClick={() => {
                  navigate("/creategame");
                }}
              >
                Create game
              </Button>
            </Grid>
          </Grid>
        </Grid>
        <br />
        <GameList searchInput={searchInput} />
      </Grid>
    </Layout>
  );
};

export default MyGamesPage;
