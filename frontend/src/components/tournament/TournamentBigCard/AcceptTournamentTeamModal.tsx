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

const AcceptTeamModal = (props: AcceptTeamModalProps) => {
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
      <DialogTitle>Accept team to a tournament</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Select team to accept to a tournament
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <TeamAutocompleteSelect
            label={"Team to accept"}
            data={teams}
            selectedTeam={acceptTeamInput}
            onSelectedTeamChange={onAcceptTeamInputChange}
          />
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button
          variant="contained"
          onClick={onSubmit}
          disabled={!acceptTeamInput}
        >
          Accept
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AcceptTeamModal;
