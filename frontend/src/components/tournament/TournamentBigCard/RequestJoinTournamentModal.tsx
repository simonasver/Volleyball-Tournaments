import React from "react";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
} from "@mui/material";
import { Team } from "../../../utils/types";

interface RequestJoinTournamentModalProps {
  errorMessage: string;
  teams: Team[];
  joinTeamInput: string;
  onJoinTournamentInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const RequestJoinTournamentModal = (props: RequestJoinTournamentModalProps) => {
  const {
    errorMessage,
    teams,
    joinTeamInput,
    onJoinTournamentInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Request join tournament</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select team to request join tournament with
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <InputLabel>Team to join</InputLabel>
          <Select
            value={joinTeamInput}
            label="Team to join"
            onChange={(e: SelectChangeEvent<string>) =>
              onJoinTournamentInputChange(e.target.value)
            }
          >
            {teams.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.title}, {item.players.length} players
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" onClick={onSubmit}>
          Join
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RequestJoinTournamentModal;
