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
import UserAutocompleteSelect from "../AutocompleteSearch/UserAutocompleteSearch";
import { User } from "../../../store/auth-slice";

interface RemoveManagerModalProps {
  errorMessage: string;
  users: User[];
  removeManagerInput: User | undefined;
  onRemoveManagerInputChange: (value: User | undefined) => void;
  searchInput: string;
  onSearchInputChange: (newValue: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const RemoveManagerModal = (props: RemoveManagerModalProps) => {
  const {
    errorMessage,
    users,
    removeManagerInput,
    onRemoveManagerInputChange,
    searchInput,
    onSearchInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Remove user from managers</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
        Select user to remove managing permissions
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <UserAutocompleteSelect
            label="User to remove from managers"
            data={users}
            selectedUser={removeManagerInput}
            onSelectedUserChange={onRemoveManagerInputChange}
            searchInput={searchInput}
            onSearchInputChange={onSearchInputChange}
          />
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button
          variant="contained"
          onClick={onSubmit}
          disabled={!removeManagerInput}
        >
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemoveManagerModal;
