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

interface AddManagerModalProps {
  errorMessage: string;
  users: User[];
  addManagerInput: User | undefined;
  onAddManagerInputChange: (value: User | undefined) => void;
  searchInput: string;
  onSearchInputChange: (newValue: string) => void;
  onSubmit: () => void;
  onClose: () => void;
}

const AddManagerModal = (props: AddManagerModalProps) => {
  const {
    errorMessage,
    users,
    addManagerInput,
    onAddManagerInputChange,
    searchInput,
    onSearchInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Add user as a manager</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
        Select user to give managing permissions for the resource
        </DialogContentText>
        <br />
        <FormControl fullWidth>
          <UserAutocompleteSelect
            label="User to make manager"
            data={users}
            selectedUser={addManagerInput}
            onSelectedUserChange={onAddManagerInputChange}
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
          disabled={!addManagerInput}
        >
          Add
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddManagerModal;
