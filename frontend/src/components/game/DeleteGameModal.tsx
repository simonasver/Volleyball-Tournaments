import {
    Alert,
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
  } from "@mui/material";
  
  interface DeleteGameModalProps {
    errorMessage: string;
    onSubmit: () => void;
    onClose: () => void;
  }
  
  const DeleteGameModal = (props: DeleteGameModalProps) => {
    const { errorMessage, onSubmit, onClose } = props;
  
    return (
      <Dialog open onClose={onClose} fullWidth>
        <DialogTitle>Delete game</DialogTitle>
        <DialogContent>
          {errorMessage && (
            <>
              <Alert severity="error">{errorMessage}</Alert>
              <br />
            </>
          )}
          <DialogContentText>
            Are you sure you want to permanently delete this game?
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
  
  export default DeleteGameModal;
  