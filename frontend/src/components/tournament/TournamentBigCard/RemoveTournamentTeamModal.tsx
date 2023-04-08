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
import { GameTeam } from "../../../utils/types";

interface RemoveTeamModalProps {
  errorMessage: string;
  teams: GameTeam[];
  removeTeamInput: string;
  onRemoveTeamInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const RemoveTeamModal = (props: RemoveTeamModalProps) => {
  const {
    errorMessage,
    teams,
    removeTeamInput,
    onRemoveTeamInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Remove team from a tournament</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select team to remove from a tournament
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <InputLabel>Team to remove</InputLabel>
          <Select
            value={removeTeamInput}
            label="Team to remove"
            onChange={(e: SelectChangeEvent<string>) =>
              onRemoveTeamInputChange(e.target.value)
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
        <Button color="error" variant="contained" onClick={onSubmit}>
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemoveTeamModal;
