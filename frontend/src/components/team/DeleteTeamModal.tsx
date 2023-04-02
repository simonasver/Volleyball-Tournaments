import React from "react";
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
import { deleteTeam } from "../../services/team.service";
import { alertActions } from "../../store/alert-slice";
import { useAppDispatch } from "../../utils/hooks";
import { useNavigate } from "react-router-dom";
import { errorMessageFromAxiosError } from "../../utils/helpers";

interface DeleteTeamModalProps {
  teamId: string;
  teamTitle: string;
  onClose: () => void;
}

const DeleteTeamModal = (props: DeleteTeamModalProps) => {
  const { teamId, teamTitle, onClose } = props;

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");

  const onDeleteSubmit = () => {
    deleteTeam(teamId)
      .then(() => {
        const successMessage = `Team ${teamTitle} was successfully deleted`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        return navigate("/myteams", { replace: true });
      })
      .catch((e) => {
        console.log(e);
        setError(errorMessageFromAxiosError(e));
      });
  };

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>Delete team</DialogTitle>
      <DialogContent>
        {error && (
          <>
            <Alert severity="error">{error}</Alert>
            <br />
          </>
        )}
        <DialogContentText>
          Are you sure you want to permanently delete team {teamTitle} and all of its
          players?
          <Typography variant="subtitle2">Team information in games will remain</Typography>
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

export default DeleteTeamModal;
