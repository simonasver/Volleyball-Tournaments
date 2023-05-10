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
} from "@mui/material";
import { Team } from "../../../utils/types";
import TeamAutocompleteSelect from "../../shared/AutocompleteSearch/TeamAutocompleteSelect";

interface RequestJoinTournamentModalProps {
  errorMessage: string;
  teams: Team[];
  joinTeamInput: Team | undefined;
  onJoinTournamentInputChange: (value: Team | undefined) => void;
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
          <TeamAutocompleteSelect
            label="Team to join with"
            data={teams}
            selectedTeam={joinTeamInput}
            onSelectedTeamChange={onJoinTournamentInputChange}
          />
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button
          variant="contained"
          onClick={onSubmit}
          disabled={!joinTeamInput}
        >
          Join
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RequestJoinTournamentModal;
