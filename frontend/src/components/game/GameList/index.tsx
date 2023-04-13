import React from "react";
import { useAppSelector } from "../../../utils/hooks";
import { useNavigate } from "react-router-dom";
import { getGames, getUserGames } from "../../../services/game.service";
import { errorMessageFromAxiosError } from "../../../utils/helpers";
import { Alert, Typography } from "@mui/material";
import Loader from "../../layout/Loader";
import GameSmallCard from "./GameSmallCard";
import { Game } from "../../../utils/types";

interface GameListProps {
  all?: boolean;
}

const GameList = (props: GameListProps) => {
  const { all } = props;

  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [games, setGames] = React.useState<Game[]>();

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!all) {
      if (!user) {
        return navigate("/", { replace: true });
      } else {
        getUserGames(user.id, abortController.signal)
          .then((res) => {
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
    } else {
      getGames(abortController.signal)
        .then((res) => {
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
    <>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Loader isOpen={isLoading} />
      {!isLoading &&
        games &&
        games.map((item) => (
          <>
            <GameSmallCard
              key={item.id}
              id={item.id}
              title={item.title}
              pictureUrl={item.pictureUrl}
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
          <br />
          <br />
          <br />
          There are no games yet. Create one!
        </Typography>
      )}
    </>
  );
};

export default GameList;
