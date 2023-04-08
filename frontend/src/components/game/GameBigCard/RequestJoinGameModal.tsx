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

interface RequestJoinGameModalProps {
  errorMessage: string;
  teams: Team[];
  joinTeamInput: string;
  onJoinGameInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const RequestJoinGameModal = (props: RequestJoinGameModalProps) => {
  const {
    errorMessage,
    teams,
    joinTeamInput,
    onJoinGameInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Request join game</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select team to request join game with
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <InputLabel>Team to join</InputLabel>
          <Select
            value={joinTeamInput}
            label="Team to join"
            onChange={(e: SelectChangeEvent<string>) =>
              onJoinGameInputChange(e.target.value)
            }
          >
            {teams.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.title}
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

export default RequestJoinGameModal;
