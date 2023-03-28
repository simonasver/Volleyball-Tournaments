import React from "react";
import { Alert, Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import BackButton from "../../components/layout/BackButton";
import { Game } from "../../utils/types";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import Loader from "../../components/layout/Loader";
import GameBigCard from "../../components/game/GameBigCard";
import { getGame } from "../../services/game.service";

const GamePage = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [game, setGame] = React.useState<Game>();

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!gameId) {
      return navigate("/", { replace: true });
    } else {
      getGame(gameId, abortController.signal)
        .then((res) => {
          console.log(res);
          setError("");
          setGame(res);
          setIsLoading(false);
        })
        .catch((e) => {
          console.log(e);
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
        <Grid item sx={{ width: { xs: "100%", md: "70%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="flex-start"
          >
            <Grid item>
              <BackButton title="All games" address="/games" />
            </Grid>
          </Grid>
          <br />
          {error && (
            <>
              <Alert severity="error">{error}</Alert>
              <br />
            </>
          )}
          <Loader isOpen={isLoading} />
          {!isLoading && game && (
            <GameBigCard
              id={game.id}
              ownerId={game.ownerId}
              title={game.title}
              description={game.description}
              createDate={new Date(game.createDate).toDateString()}
              status={game.status}
              firstTeam={game.firstTeam}
              secondTeam={game.secondTeam}
              firstTeamScore={game.firstTeamScore}
              secondTeamScore={game.secondTeamScore}
              requestedTeams={game.requestedTeams}
            />
          )}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default GamePage;
