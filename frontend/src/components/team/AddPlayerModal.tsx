import React from "react";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
} from "@mui/material";

interface AddPlayerModalProps {
  errorMessage: string;
  searchInput: string;
  onSearchInputChange: (value: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const AddPlayerModal = (props: AddPlayerModalProps) => {
  const { errorMessage, searchInput, onSearchInputChange, onSubmit, onClose } =
    props;

  return (
    <Dialog
      open
      onClose={onClose}
    >
      <DialogTitle>Add player</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>Enter new player name</DialogContentText>
        <br />
        <TextField
          value={searchInput}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            onSearchInputChange(e.target.value)
          }
          type="text"
          label="New player name"
          variant="outlined"
          fullWidth
          required
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" onClick={onSubmit}>
          Add
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddPlayerModal;
