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
import { GameTeam } from "../../../utils/types";
import GameTeamAutocompleteSelect from "../../shared/AutocompleteSearch/GameTeamAutocompleteSelect";

interface RemoveTeamModalProps {
  errorMessage: string;
  teams: GameTeam[];
  removeTeamInput: GameTeam | undefined;
  onRemoveTeamInputChange: (value: GameTeam | undefined) => void;
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
          <GameTeamAutocompleteSelect
            label={"Team to remove"}
            data={teams}
            selectedTeam={removeTeamInput}
            onSelectedTeamChange={onRemoveTeamInputChange}
          />
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button
          color="error"
          variant="contained"
          onClick={onSubmit}
          disabled={!removeTeamInput}
        >
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemoveTeamModal;
