import React from "react";
import {
  Alert,
  Button,
  FormControlLabel,
  FormGroup,
  Grid,
  Switch,
  Tab,
  Tabs,
  TextField,
  Typography,
} from "@mui/material";
import BackButton from "../layout/BackButton";
import { addGame } from "../../services/game.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useNavigate } from "react-router-dom";
import { alertActions } from "../../store/alert-slice";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { GamePreset } from "../../utils/types";

const CreateGameForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");

  const [title, setTitle] = React.useState("");
  const [pictureUrl, setPictureUrl] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [isBasic, setIsBasic] = React.useState(true);
  const [pointsToWin, setPointsToWin] = React.useState(25);
  const [pointsToWinLastSet, setPointsToWinLastSet] = React.useState(15);
  const [pointDifferenceToWin, setPointDifferenceToWin] = React.useState(2);
  const [maxSets, setMaxSets] = React.useState(5);
  const [limitPlayers, setLimitPlayers] = React.useState(true);
  const [playersPerTeam, setPlayersPerTeam] = React.useState(6);
  const [isPrivate, setIsPrivate] = React.useState(false);

  const user = useAppSelector((state) => state.auth.user);

  const [currentPreset, setCurrentPreset] = React.useState(GamePreset.Regular);

  React.useEffect(() => {
    if (!user) {
      return navigate("/", { replace: true });
    }
  }, []);

  const onChangePreset = (event: React.SyntheticEvent, newValue: number) => {
    switch(newValue) {
      case 0:
        setCurrentPreset(GamePreset.Regular);
        setPointsToWin(25);
        setPointsToWinLastSet(15);
        setPointDifferenceToWin(2);
        setMaxSets(5);
        setPlayersPerTeam(6);
        break;
      case 1:
        setCurrentPreset(GamePreset.Beach);
        setPointsToWin(21);
        setPointsToWinLastSet(15);
        setPointDifferenceToWin(2);
        setMaxSets(3);
        setPlayersPerTeam(2);
        break;
      case 2:
        setCurrentPreset(GamePreset.None);
        break;
    }
  };

  const onSubmitHandler = (event: React.FormEvent) => {
    event.preventDefault();
    addGame(
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
      .then((res) => {
        const successMessage = `Game ${title} was successfully created`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        return navigate(`/game/${res}`, { replace: true });
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
          <BackButton title="My games" address="/mygames" />
        </Grid>
      </Grid>
      <br />
      <Typography variant="h5">Create Game</Typography>
      <br />
      <Typography variant="subtitle2">
        Enter your new game information!
      </Typography>
      <Typography variant="subtitle2">You can add teams later.</Typography>
      <br />
      <form onSubmit={onSubmitHandler}>
        <FormGroup>
          <FormControlLabel
            checked={isPrivate}
            control={
              <Switch
                value={isPrivate}
                onChange={() => setIsPrivate((state) => !state)}
              />
            }
            label="Private game"
          />
        </FormGroup>
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
          />
        </FormGroup>
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
          type="text"
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
        <Tabs value={currentPreset} onChange={onChangePreset}>
          <Tab label="Regular" />
          <Tab label="Beach" />
          <Tab label="Custom" />
        </Tabs>
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
          disabled={currentPreset !== GamePreset.None}
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
          disabled={currentPreset !== GamePreset.None}
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
          disabled={currentPreset !== GamePreset.None}
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
          disabled={currentPreset !== GamePreset.None}
        />
        <br />
        <br />
        <FormGroup>
          <FormControlLabel
            checked={limitPlayers}
            control={
              <Switch
                value={limitPlayers}
                onChange={() => setLimitPlayers((state) => !state)}
              />
            }
            label="Limit players (player count in teams must be equal to a set number)"
            disabled={currentPreset !== GamePreset.None}
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
          disabled={!limitPlayers || currentPreset !== GamePreset.None}
          fullWidth
        />
        <br />
        <br />
        <Grid item xs={12}>
          <Button
            variant="contained"
            type="submit"
            sx={{ width: { xs: "100%", md: "inherit" } }}
          >
            Create
          </Button>
        </Grid>
      </form>
      <br />
    </Grid>
  );
};

export default CreateGameForm;
