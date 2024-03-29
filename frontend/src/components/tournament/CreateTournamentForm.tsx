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
import { GamePreset } from "../../utils/types";
import { addTournament } from "../../services/tournament.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { alertActions } from "../../store/alert-slice";

const CreateTournamentForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");

  const [title, setTitle] = React.useState("");
  const [pictureUrl, setPictureUrl] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [isBasic, setIsBasic] = React.useState(true);
  const [singleThirdPlace, setSingleThirdPlace] = React.useState(false);
  const [maxTeams, setMaxTeams] = React.useState(128);
  const [pointsToWin, setPointsToWin] = React.useState(25);
  const [pointsToWinLastSet, setPointsToWinLastSet] = React.useState(15);
  const [pointDifferenceToWin, setPointDifferenceToWin] = React.useState(2);
  const [maxSets, setMaxSets] = React.useState(5);
  const [limitPlayers, setLimitPlayers] = React.useState(true);
  const [playersPerTeam, setPlayersPerTeam] = React.useState(6);
  const [isPrivate, setIsPrivate] = React.useState(false);

  const [currentTab, setCurrentTab] = React.useState<number>(0);

  const [currentPreset, setCurrentPreset] = React.useState(GamePreset.Indoors);

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
      return navigate("/", { replace: true });
    }
  }, []);

  const onTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue);
  };

  const onChangePreset = (event: React.SyntheticEvent, newValue: number) => {
    switch (newValue) {
      case 0:
        setCurrentPreset(GamePreset.Indoors);
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
    addTournament(
      title,
      pictureUrl,
      description,
      isBasic,
      singleThirdPlace,
      maxTeams,
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
        return navigate(`/tournament/${res}`, { replace: true });
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
          <BackButton title="My tournaments" address="/mytournaments" />
        </Grid>
      </Grid>
      <br />
      <Typography variant="h5">Create Tournament</Typography>
      <br />
      <Typography variant="subtitle2">
        Enter your new tournament information!
      </Typography>
      <Typography variant="subtitle2">You can add teams later.</Typography>
      <br />
      <form onSubmit={onSubmitHandler}>
        <Tabs
          value={currentTab}
          onChange={onTabChange}
          variant="fullWidth"
          centered
        >
          <Tab value={0} label="Tournament settings" />
          <Tab value={1} label="Games settings" />
        </Tabs>
        <br />
        <div hidden={currentTab !== 0}>
          <FormGroup>
            <FormControlLabel
              checked={isPrivate}
              control={
                <Switch
                  value={isPrivate}
                  onChange={() => setIsPrivate((state) => !state)}
                />
              }
              label="Private tournament"
            />
          </FormGroup>
          <br />
          <FormGroup>
            <FormControlLabel
              checked={singleThirdPlace}
              control={
                <Switch
                  value={singleThirdPlace}
                  onChange={() => setSingleThirdPlace((state) => !state)}
                />
              }
              label="Match for third place"
            />
          </FormGroup>
          <br />
          <TextField
            value={title}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setTitle(e.target.value)
            }
            type="text"
            label="Tournament title"
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
            label="Tournament picture (url)"
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
            label="Tournament description"
            variant="outlined"
            fullWidth
          />
          <br />
          <br />
          <TextField
            value={maxTeams}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setMaxTeams(parseInt(e.target.value) ?? 2)
            }
            type="number"
            label="Team limit"
            variant="outlined"
            inputProps={{ min: 2, max: 128 }}
            fullWidth
          />
          <br />
          <br />
        </div>
        <div hidden={currentTab !== 1}>
          <br />
          <Tabs value={currentPreset} onChange={onChangePreset}>
            <Tab label="Indoors" />
            <Tab label="Beach" />
            <Tab label="Custom" />
          </Tabs>
          <br />
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
              setMaxSets(parseInt(e.target.value) ?? 0)
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
              setPlayersPerTeam(parseInt(e.target.value) ?? 0)
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
        </div>

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

export default CreateTournamentForm;
