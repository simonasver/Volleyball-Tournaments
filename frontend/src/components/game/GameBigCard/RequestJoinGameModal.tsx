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

interface RequestJoinGameModalProps {
  errorMessage: string;
  teams: Team[];
  joinTeamInput: Team | undefined;
  onJoinGameInputChange: (value: Team | undefined) => void;
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
          <TeamAutocompleteSelect label={"Team to join with"} data={teams} selectedTeam={joinTeamInput} onSelectedTeamChange={onJoinGameInputChange} />
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
