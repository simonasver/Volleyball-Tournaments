import React from "react";
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, TextField } from "@mui/material";

interface GenerateTournamentModalProps {
    onSubmit: (amount: number) => void;
    onClose: () => void;
}

const GenerateTournamentModal = (props: GenerateTournamentModalProps) => {
    const { onSubmit, onClose } = props;

    const [teamAmount, setTeamAmount] = React.useState(2);

    return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Generate tournament</DialogTitle>
      <DialogContent>
        <DialogContentText>Enter the amount of teams to be in the generated tournament</DialogContentText>
        <br />
        <TextField
          value={teamAmount}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTeamAmount(parseInt(e.target.value))
          }
          type="number"
          label="Team amount"
          variant="outlined"
          fullWidth
          required
          autoFocus
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" onClick={() => onSubmit(teamAmount)}>
          Generate
        </Button>
      </DialogActions>
    </Dialog>
    )
};

export default GenerateTournamentModal;