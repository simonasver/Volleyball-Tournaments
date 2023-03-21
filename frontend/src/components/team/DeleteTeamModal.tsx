import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Typography,
} from "@mui/material";

interface DeleteTeamModalProps {
  errorMessage: string;
  onSubmit: () => void;
  onClose: () => void;
}

const DeleteTeamModal = (props: DeleteTeamModalProps) => {
  const { errorMessage, onSubmit, onClose } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Delete team</DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Are you sure you want to permanently delete this team and all of its
          players?
          <Typography variant="subtitle2">Team information in games will remain</Typography>
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
