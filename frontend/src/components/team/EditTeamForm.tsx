import React from "react";
import { Alert, Button, Grid, TextField, Typography } from "@mui/material";
import BackButton from "../layout/BackButton";
import { useNavigate, useParams } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import { editTeam, getTeam } from "../../services/team.service";
import axios from "axios";

const EditTeamForm = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();

  const [error, setError] = React.useState("");

  const [teamName, setTeamName] = React.useState("");
  const [teamPicture, setTeamPicture] = React.useState("");
  const [teamDescription, setTeamDescription] = React.useState("");

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!teamId) {
      return navigate("/", { replace: true });
    }
    if (!user) {
      return navigate("/", { replace: true });
    }
    getTeam(teamId)
      .then((res) => {
        setError("");
        setTeamName(res.title);
        setTeamPicture(res.pictureUrl);
        setTeamDescription(res.description);
      })
      .catch((e) => {
        if (!axios.isCancel(e)) {
          console.log(e);
          if (e.response) {
            setError(e.response.data || "Error");
          } else if (e.request) {
            setError("Connection error");
          } else {
            setError("Error");
          }
        }
      });
  }, []);

  const onEditTeamSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!teamId) {
      return navigate("/", { replace: true });
    }
    editTeam(teamId, teamName, teamPicture, teamDescription)
      .then(() => {
        navigate("/myteams", { replace: true });
      })
      .catch((e) => {
        if (!axios.isCancel(e)) {
          console.log(e);
          if (e.response) {
            setError(e.response.data || "Error");
          } else if (e.request) {
            setError("Connection error");
          } else {
            setError("Error");
          }
        }
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
          <BackButton />
        </Grid>
      </Grid>
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
