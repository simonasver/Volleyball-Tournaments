import React from "react";
import {
    Alert,
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
  } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { alertActions } from "../../../store/alert-slice";
import { useAppDispatch } from "../../../utils/hooks";
import { errorMessageFromAxiosError } from "../../../utils/helpers";
import { deleteTournament } from "../../../services/tournament.service";
  
  interface DeleteTournamentModalProps {
    tournamentId: string;
    tournamentTitle: string;
    onClose: () => void;
  }
  
  const DeleteTournamentModal = (props: DeleteTournamentModalProps) => {
    const { tournamentId, tournamentTitle, onClose } = props;

    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [error, setError] = React.useState("");

    const onDeleteSubmit = () => {
      deleteTournament(tournamentId)
        .then(() => {
          const successMessage = `Tournament ${tournamentTitle} was deleted`;
          dispatch(
            alertActions.changeAlert({ type: "success", message: successMessage })
          );
          return navigate("/mytournaments", { replace: true });
        })
        .catch((e) => {
          console.log(e);
          setError(errorMessageFromAxiosError(e));
        });
    };
  
    return (
      <Dialog open onClose={onClose} fullWidth>
        <DialogTitle>Delete tournament</DialogTitle>
        <DialogContent>
          {error && (
            <>
              <Alert severity="error">{error}</Alert>
              <br />
            </>
          )}
          <DialogContentText>
            Are you sure you want to permanently delete tournament {tournamentTitle}?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Close</Button>
          <Button variant="contained" color="error" onClick={onDeleteSubmit}>
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    );
  };
  
  export default DeleteTournamentModal;
  