import React from "react";
import { Alert, Button, Grid, TextField, Typography } from "@mui/material";

const CreateTeamForm = () => {
    const [error, setError] = React.useState("");

    const [teamName, setTeamName] = React.useState("");
    const [teamDescription, setTeamDescription] = React.useState("");

    const onCreateTeamSubmit = (event: React.FormEvent) => {
        event.preventDefault();
    };
    
  return (
    <Grid item xs={3}>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Typography variant="h5">Create Team</Typography>
      <br />
      <Typography variant="subtitle2">
        Enter your new team information!
      </Typography>
      <Typography variant="subtitle2">
        You can add players later.
      </Typography>
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
        <Button variant="contained" type="submit" fullWidth>
          Create
        </Button>
      </form>
      <br />
    </Grid>
  );
};

export default CreateTeamForm;
