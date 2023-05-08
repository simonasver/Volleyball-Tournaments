import React from "react";
import { Alert, Button, Grid, TextField, Typography } from "@mui/material";
import BackButton from "../layout/BackButton";
import { useNavigate, useParams } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { editTeam, getTeam } from "../../services/team.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import Loader from "../layout/Loader";
import { alertActions } from "../../store/alert-slice";

const EditTeamForm = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [teamName, setTeamName] = React.useState("");
  const [teamPicture, setTeamPicture] = React.useState("");
  const [teamDescription, setTeamDescription] = React.useState("");

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!teamId) {
      return navigate("/", { replace: true });
    } else if (!user) {
      return navigate("/", { replace: true });
    } else {
      getTeam(teamId, abortController.signal)
        .then((res) => {
          setError("");
          setTeamName(res.title);
          setTeamPicture(res.pictureUrl);
          setTeamDescription(res.description);
          setIsLoading(false);
        })
        .catch((e) => {
          console.log(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if(errorMessage){
            setIsLoading(false);
          }
        })
    }
    return () => abortController.abort();
  }, []);

  const onEditTeamSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!teamId) {
      return navigate("/", { replace: true });
    }
    editTeam(teamId, teamName, teamPicture, teamDescription)
      .then(() => {
        const successMessage = `Team ${teamName} was successfully updated`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        return navigate(`/team/${teamId}`, { replace: true });
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
          <BackButton title="Team" address={`/team/${teamId}`} />
        </Grid>
      </Grid>
      <Loader isOpen={isLoading}/>
      <br />
      <Typography variant="h5">Edit Team</Typography>
      <br />
      <Typography variant="subtitle2">Change your team information!</Typography>
      <br />
      <form onSubmit={onEditTeamSubmit}>
        <TextField
          value={teamName}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTeamName(e.target.value)
          }
          type="text"
          label="Team name"
          variant="outlined"
          fullWidth
          required
        />
        <br />
        <br />
        <TextField
          value={teamPicture}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTeamPicture(e.target.value)
          }
          type="text"
          label="Team picture (url)"
          variant="outlined"
          fullWidth
        />
        <br />
        <br />
        <TextField
          value={teamDescription}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTeamDescription(e.target.value)
          }
          type="text"
          label="Team description"
          variant="outlined"
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
            Save
          </Button>
        </Grid>
      </form>
      <br />
    </Grid>
  );
};

export default EditTeamForm;
