import React from "react";
import { Alert, Button, Grid, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import BackButton from "../../components/layout/BackButton";
import Layout from "../../components/layout/Layout";
import Loader from "../../components/layout/Loader";
import { Game } from "../../utils/types";
import GameSmallCard from "../../components/game/GameSmallCard";
import { useAppSelector } from "../../utils/hooks";
import { getUserGames } from "../../services/game.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";

const MyGamesPage = () => {
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [games, setGames] = React.useState<Game[]>();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      getUserGames(user.id, abortController.signal)
        .then((res) => {
          console.log(res);
          setGames(res);
          setIsLoading(false);
        })
        .catch((e) => {
          console.error(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if (errorMessage) {
            setIsLoading(false);
          }
        });
    }

    return () => abortController.abort();
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
        <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="space-between"
          >
            <Grid item>
              <BackButton title="All games" address="/games" />
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
        <br />
        {error && (
          <>
            <Alert severity="error">{error}</Alert>
            <br />
          </>
        )}
        <Typography variant="h3">My games</Typography>
        <br />
        <Loader isOpen={isLoading} />
        {!isLoading &&
          games &&
          games.map((item) => (
            <>
              <GameSmallCard
                key={item.id}
                id={item.id}
                title={item.title}
                description={item.description}
                createDate={new Date(item.createDate).toDateString()}
                status={item.status}
                onButtonPress={() => navigate("/game/" + item.id)}
              />
              <br />
            </>
          ))}
        {!isLoading && (!games || (games && games.length === 0)) && (
          <Typography variant="h6">There are no yet. Create one!</Typography>
        )}
      </Grid>
    </Layout>
  );
};

export default MyGamesPage;
