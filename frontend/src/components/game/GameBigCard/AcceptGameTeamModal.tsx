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

interface AcceptTeamModalProps {
  errorMessage: string;
  teams: Team[];
  acceptTeamInput: Team | undefined;
  onAcceptTeamInputChange: (value: Team | undefined) => void;
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
          <TeamAutocompleteSelect label={"Team to accept"} data={teams} selectedTeam={acceptTeamInput} onSelectedTeamChange={onAcceptTeamInputChange} />
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
