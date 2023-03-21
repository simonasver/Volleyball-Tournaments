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
import { TeamPlayer } from "../../utils/types";

interface Player {
  [key: string]: { label: string; isChecked: boolean };
}

const players: Player = {
  "1": { label: "Simonas", isChecked: false },
  "2": { label: "Petras", isChecked: false },
  "3": { label: "Jonas", isChecked: false },
};

interface RemovePlayerModalProps {
  errorMessage: string;
  players: TeamPlayer[];
  removePlayerInput: string;
  onRemovePlayerInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const RemovePlayerModal = (props: RemovePlayerModalProps) => {
  const {
    errorMessage,
    players,
    removePlayerInput,
    onRemovePlayerInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Remove players</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select player to remove from the team
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <InputLabel>Player to remove</InputLabel>
          <Select
            value={removePlayerInput}
            label="Player to remove"
            onChange={(e: SelectChangeEvent<string>) =>
              onRemovePlayerInputChange(e.target.value)
            }
          >
            {players.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" onClick={onSubmit}>
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemovePlayerModal;
