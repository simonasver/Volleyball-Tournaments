import { Alert, Button, Grid, Typography } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import GameSmallCard from "../../components/game/GameSmallCard";
import BackButton from "../../components/layout/BackButton";
import Layout from "../../components/layout/Layout";
import Loader from "../../components/layout/Loader";
import { getGames } from "../../services/game.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { Game } from "../../utils/types";

const AllGamesPage = () => {
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [games, setGames] = React.useState<Game[]>();

  React.useEffect(() => {
    const abortController = new AbortController();
    getGames(abortController.signal)
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
              <BackButton address="/" />
            </Grid>
            <Grid item>
              <Button
                variant="contained"
                onClick={() => {
                  navigate("/mygames");
                }}
              >
                My games
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
        <Typography variant="h3">All public games</Typography>
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
          <Typography variant="h6">
            There are no public games yet. Create one!
          </Typography>
        )}
      </Grid>
    </Layout>
  );
};

export default AllGamesPage;
