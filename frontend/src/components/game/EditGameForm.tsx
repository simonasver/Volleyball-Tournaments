import React from "react";
import {
  Alert,
  Button,
  FormControlLabel,
  FormGroup,
  Grid,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import BackButton from "../layout/BackButton";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { editGame, getGame } from "../../services/game.service";
import Loader from "../layout/Loader";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { GameStatus } from "../../utils/types";
import { alertActions } from "../../store/alert-slice";

const EditGameForm = () => {
  const { gameId } = useParams();

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [title, setTitle] = React.useState("");
  const [pictureUrl, setPictureUrl] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [isBasic, setIsBasic] = React.useState(true);
  const [pointsToWin, setPointsToWin] = React.useState(25);
  const [pointsToWinLastSet, setPointsToWinLastSet] = React.useState(15);
  const [pointDifferenceToWin, setPointDifferenceToWin] = React.useState(2);
  const [maxSets, setMaxSets] = React.useState(5);
  const [limitPlayers, setLimitPlayers] = React.useState(true);
  const [playersPerTeam, setPlayersPerTeam] = React.useState(5);
  const [isPrivate, setIsPrivate] = React.useState(false);

  const [isTournamentGame, setIsTournamentGame] = React.useState(false);

  const [gameStatus, setGameStatus] = React.useState(0);

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!gameId) {
      return navigate("/", { replace: true });
    } else if (!user) {
      return navigate("/", { replace: true });
    } else {
      getGame(gameId, abortController.signal)
        .then((res) => {
          setError("");
          setTitle(res.title);
          setPictureUrl(res.pictureUrl);
          setDescription(res.description);
          setIsBasic(res.basic);
          setPointsToWin(res.pointsToWin);
          setPointsToWinLastSet(res.pointsToWinLastSet);
          setPointDifferenceToWin(res.pointDifferenceToWin);
          setMaxSets(res.maxSets);
          setPlayersPerTeam(res.playersPerTeam);
          setLimitPlayers(res.playersPerTeam !== 0);
          setIsPrivate(res.isPrivate);
          setGameStatus(res.status);

          setIsTournamentGame(!!res.tournamentMatch);

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

  const onSubmitHandler = (event: React.FormEvent) => {
    event.preventDefault();
    if (!gameId) {
      return navigate("/", { replace: true });
    }
    if(!isTournamentGame) {
      editGame(
        gameId,
        title,
        pictureUrl,
        description,
        isBasic,
        pointsToWin,
        pointsToWinLastSet,
        pointDifferenceToWin,
        maxSets,
        limitPlayers ? playersPerTeam : 0,
        isPrivate
      )
        .then(() => {
          const successMessage = `Game ${title} was successfully updated`;
          dispatch(
            alertActions.changeAlert({ type: "success", message: successMessage })
          );
          return navigate(`/game/${gameId}`, { replace: true });
        })
        .catch((e) => {
          console.log(e);
          setError(errorMessageFromAxiosError(e));
          dispatch(
            alertActions.changeAlert({
              type: "error",
              message: errorMessageFromAxiosError(e),
            })
          );
        });
    } else {
      editGame(
        gameId,
        title,
        pictureUrl,
        description,
      )
        .then(() => {
          const successMessage = `Game ${title} was successfully updated`;
          dispatch(
            alertActions.changeAlert({ type: "success", message: successMessage })
          );
          return navigate(`/game/${gameId}`, { replace: true });
        })
        .catch((e) => {
          console.log(e);
          setError(errorMessageFromAxiosError(e));
          dispatch(
            alertActions.changeAlert({
              type: "error",
              message: errorMessageFromAxiosError(e),
            })
          );
        });
    }
  };

  return (
    <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Grid
        container
        spacing={1}
        direction="row"
        alignItems="center"
        justifyContent="flex-start"
      >
        <Grid item>
          <BackButton title="Game" address={`/game/${gameId}`} />
        </Grid>
      </Grid>
      <Loader isOpen={isLoading} />
      <br />
      <Typography variant="h5">Edit Game</Typography>
      <br />
      <Typography variant="subtitle2">Change your game information!</Typography>
      <br />
      <form onSubmit={onSubmitHandler}>
        {!isTournamentGame && (
          <FormGroup>
            <FormControlLabel
              control={
                <Switch
                  checked={isPrivate}
                  onChange={() => setIsPrivate((state) => !state)}
                />
              }
              label="Private game"
            />
          </FormGroup>
        )}
        {!isTournamentGame && (
          <>
            <FormGroup>
              <FormControlLabel
                checked={!isBasic}
                control={
                  <Switch
                    value={!isBasic}
                    onChange={() => setIsBasic((state) => !state)}
                  />
                }
                label="Game player scoreboard has extended options"
                disabled={gameStatus >= GameStatus.Started}
              />
            </FormGroup>
            <br />
          </>
        )}
        <br />
        <TextField
          value={title}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTitle(e.target.value)
          }
          type="text"
          label="Game title"
          variant="outlined"
          fullWidth
          required
        />
        <br />
        <br />
        <TextField
          value={pictureUrl}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setPictureUrl(e.target.value)
          }
          type="url"
          label="Game picture (url)"
          variant="outlined"
          fullWidth
        />
        <br />
        <br />
        <TextField
          value={description}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setDescription(e.target.value)
          }
          type="text"
          label="Game description"
          variant="outlined"
          fullWidth
        />
        <br />
        <br />
        <TextField
          value={pointsToWin}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setPointsToWin(parseInt(e.target.value) ?? 1)
          }
          type="number"
          label="Points needed to win a set"
          variant="outlined"
          inputProps={{ min: 1 }}
          fullWidth
          disabled={gameStatus >= GameStatus.Started || isTournamentGame}
        />
        <br />
        <br />
        <TextField
          value={pointsToWinLastSet}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setPointsToWinLastSet(parseInt(e.target.value) ?? 0)
          }
          type="number"
          label="Points needed to win the last set"
          variant="outlined"
          inputProps={{ min: 0 }}
          fullWidth
          disabled={gameStatus >= GameStatus.Started || isTournamentGame}
        />
        <br />
        <br />
        <TextField
          value={pointDifferenceToWin}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setPointDifferenceToWin(parseInt(e.target.value) ?? 0)
          }
          type="number"
          label="Point difference needed to win a set"
          variant="outlined"
          inputProps={{ min: 0, max: 10 }}
          fullWidth
          disabled={gameStatus >= GameStatus.Started || isTournamentGame}
        />
        <br />
        <br />
        <TextField
          value={maxSets}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setMaxSets(parseInt(e.target.value) ?? 1)
          }
          type="number"
          label="Best of x (max amount of sets)"
          variant="outlined"
          inputProps={{ min: 1, max: 5 }}
          fullWidth
          disabled={gameStatus >= GameStatus.Started || isTournamentGame}
        />
        <br />
        <br />
        {!isTournamentGame && (
          <>
            <FormGroup>
              <FormControlLabel
                checked={limitPlayers}
                control={
                  <Switch
                    value={limitPlayers}
                    onChange={() => setLimitPlayers((state) => !state)}
                    disabled={gameStatus > GameStatus.New}
                  />
                }
                label="Limit players (player count in both teams must be equal)"
              />
            </FormGroup>
            <br />
            <TextField
              value={playersPerTeam}
              onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
                setPlayersPerTeam(parseInt(e.target.value) ?? 1)
              }
              type="number"
              label="Amount of players in each team"
              variant="outlined"
              inputProps={{ min: 1, max: 12 }}
              fullWidth
              disabled={gameStatus > GameStatus.New || !limitPlayers}
            />
            <br />
          </>
        )}
        <br />
        <Grid item xs={12}>
          <Button
            variant="contained"
            type="submit"
            sx={{ width: { xs: "100%", md: "inherit" } }}
          >
            Save
          </Button>
        </Grid>
      </form>
      <br />
    </Grid>
  );
};

export default EditGameForm;
