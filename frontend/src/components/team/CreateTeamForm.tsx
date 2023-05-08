import React from "react";
import { Alert, Button, Grid, TextField, Typography } from "@mui/material";
import BackButton from "../layout/BackButton";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { useNavigate } from "react-router-dom";
import { addTeam } from "../../services/team.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { alertActions } from "../../store/alert-slice";

const CreateTeamForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");

  const [teamName, setTeamName] = React.useState("");
  const [teamPicture, setTeamPicture] = React.useState("");
  const [teamDescription, setTeamDescription] = React.useState("");

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
      return navigate("/", { replace: true });
    }
  }, []);

  const onCreateTeamSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    addTeam(teamName, teamPicture, teamDescription)
      .then((res) => {
        const successMessage = `Team ${teamName} was succesfully created`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        return navigate(`/team/${res}`, { replace: true });
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
          <BackButton title="My teams" address="/myteams" />
        </Grid>
      </Grid>
      <br />
      <Typography variant="h5">Create Team</Typography>
      <br />
      <Typography variant="subtitle2">
        Enter your new team information!
      </Typography>
      <Typography variant="subtitle2">You can add players later.</Typography>
      <br />
      <form onSubmit={onCreateTeamSubmit}>
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
            Create
          </Button>
        </Grid>
      </form>
      <br />
    </Grid>
  );
};

export default CreateTeamForm;
