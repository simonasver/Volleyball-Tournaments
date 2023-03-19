import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";

interface DeleteTeamModalProps {
  errorMessage: string;
  onSubmit: () => void;
  onClose: () => void;
}

const DeleteTeamModal = (props: DeleteTeamModalProps) => {
  const { errorMessage, onSubmit, onClose } = props;

  return (
    <Dialog open onClose={onClose}>
      <DialogTitle>Delete team</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Are you sure you want to permanently delete this team and all of its players?
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button variant="contained" color="error" onClick={onSubmit}>
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteTeamModal;
