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
import { TournamentStatus } from "../../utils/types";
import {
  editTournament,
  getTournament,
} from "../../services/tournament.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useNavigate, useParams } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { alertActions } from "../../store/alert-slice";
import Loader from "../layout/Loader";

const EditTournamentForm = () => {
  const { tournamentId } = useParams();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [status, setStatus] = React.useState<TournamentStatus>(
    TournamentStatus.Open
  );
  const [title, setTitle] = React.useState("");
  const [pictureUrl, setPictureUrl] = React.useState("");
  const [isBasic, setIsBasic] = React.useState(true);
  const [description, setDescription] = React.useState("");
  const [maxTeams, setMaxTeams] = React.useState(128);
  const [pointsToWin, setPointsToWin] = React.useState(25);
  const [pointsToWinLastSet, setPointsToWinLastSet] = React.useState(15);
  const [pointDifferenceToWin, setPointDifferenceToWin] = React.useState(2);
  const [maxSets, setMaxSets] = React.useState(5);
  const [limitPlayers, setLimitPlayers] = React.useState(true);
  const [playersPerTeam, setPlayersPerTeam] = React.useState(6);
  const [isPrivate, setIsPrivate] = React.useState(false);

  const [currentTab, setCurrentTab] = React.useState<number>(0);

  const onTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue);
  };

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!tournamentId) {
      return navigate("/", { replace: true });
    } else if (!user) {
      return navigate("/", { replace: true });
    } else {
      getTournament(tournamentId, abortController.signal)
        .then((res) => {
          setError("");
          setTitle(res.title);
          setPictureUrl(res.pictureUrl);
          setDescription(res.description);
          setIsBasic(res.basic);
          setMaxTeams(res.maxTeams);
          setPointsToWin(res.pointsToWin);
          setPointsToWinLastSet(res.pointsToWinLastSet);
          setPointDifferenceToWin(res.pointDifferenceToWin);
          setMaxSets(res.maxSets);
          setLimitPlayers(res.playersPerTeam !== 0);
          setPlayersPerTeam(res.playersPerTeam);
          setIsPrivate(res.isPrivate);

          setStatus(res.status);

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
    if (!tournamentId) {
      return navigate("/", { replace: true });
    }
    editTournament(
      tournamentId,
      title,
      pictureUrl,
      description,
      isBasic,
      maxTeams,
      pointsToWin,
      pointsToWinLastSet,
      pointDifferenceToWin,
      maxSets,
      limitPlayers ? playersPerTeam : 0,
      isPrivate
    )
      .then(() => {
        const successMessage = `Tournament ${title} was successfully updated`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        return navigate(`/tournament/${tournamentId}`, { replace: true });
      })
      .catch((e) => {
        console.log(e);
        setError(errorMessageFromAxiosError(e));
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
      <Loader isOpen={isLoading} />
      <Grid
        container
        spacing={1}
        direction="row"
        alignItems="center"
        justifyContent="flex-start"
      >
        <Grid item>
          <BackButton
            title="Tournament"
            address={`/tournament/${tournamentId}`}
          />
        </Grid>
      </Grid>
      <br />
      <Typography variant="h5">Edit Tournament</Typography>
      <br />
      <Typography variant="subtitle2">
        Edit your tournament information!
      </Typography>
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
            type="text"
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
            disabled={status >= TournamentStatus.Started}
          />
          <br />
          <br />
        </div>
        <div hidden={currentTab !== 1}>
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
            disabled={status >= TournamentStatus.Started}
          />
          <br />
          <br />
          <TextField
            value={pointsToWinLastSet}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setPointsToWinLastSet(parseInt(e.target.value) ?? 1)
            }
            type="number"
            label="Points needed to win the last set"
            variant="outlined"
            inputProps={{ min: 0 }}
            fullWidth
            disabled={status >= TournamentStatus.Started}
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
            disabled={status >= TournamentStatus.Started}
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
            disabled={status >= TournamentStatus.Started}
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
              disabled={status >= TournamentStatus.Started}
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
            disabled={!limitPlayers || status >= TournamentStatus.Started}
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
            Edit
          </Button>
        </Grid>
      </form>
      <br />
    </Grid>
  );
};

export default EditTournamentForm;
