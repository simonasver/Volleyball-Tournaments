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
import { deleteGame } from "../../services/game.service";
import { useNavigate } from "react-router-dom";
import { alertActions } from "../../store/alert-slice";
import { useAppDispatch } from "../../utils/hooks";
import { errorMessageFromAxiosError } from "../../utils/helpers";
  
  interface DeleteGameModalProps {
    gameId: string;
    gameTitle: string;
    onClose: () => void;
  }
  
  const DeleteGameModal = (props: DeleteGameModalProps) => {
    const { gameId, gameTitle, onClose } = props;

    const navigate = useNavigate();
    const dispatch = useAppDispatch();

    const [error, setError] = React.useState("");

    const onDeleteSubmit = () => {
      deleteGame(gameId)
        .then(() => {
          const successMessage = `Game ${gameTitle} was deleted`;
          dispatch(
            alertActions.changeAlert({ type: "success", message: successMessage })
          );
          return navigate("/mygames", { replace: true });
        })
        .catch((e) => {
          console.log(e);
          setError(errorMessageFromAxiosError(e));
        });
    };
  
    return (
      <Dialog open onClose={onClose} fullWidth>
        <DialogTitle>Delete game</DialogTitle>
        <DialogContent>
          {error && (
            <>
              <Alert severity="error">{error}</Alert>
              <br />
            </>
          )}
          <DialogContentText>
            Are you sure you want to permanently delete game {gameTitle}?
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
  
  export default DeleteGameModal;
  