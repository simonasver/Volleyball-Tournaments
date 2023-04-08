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

interface AcceptTeamModalProps {
  errorMessage: string;
  teams: Team[];
  acceptTeamInput: string;
  onAcceptTeamInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const AcceptGameTeamModal = (props: AcceptTeamModalProps) => {
  const {
    errorMessage,
    teams,
    acceptTeamInput,
    onAcceptTeamInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Accept team to a game</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select team to accept to a game
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <InputLabel>Team to accept</InputLabel>
          <Select
            value={acceptTeamInput}
            label="Team to accept"
            onChange={(e: SelectChangeEvent<string>) =>
              onAcceptTeamInputChange(e.target.value)
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
          Accept
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AcceptGameTeamModal;
