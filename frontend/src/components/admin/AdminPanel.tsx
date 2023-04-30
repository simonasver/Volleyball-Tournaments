import React from "react";
import { Button, Stack, Typography } from "@mui/material";
import { alertActions } from "../../store/alert-slice";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useAppDispatch } from "../../utils/hooks";
import GenerateTournamentModal from "./GenerateTournamentsModal";
import { useNavigate } from "react-router-dom";
import { generateTournament } from "../../services/tournament.service";

enum Modal {
  None = 0,
  Generate = 1,
}

const AdminPanel = () => {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const closeModals = () => {
    setModalStatus(Modal.None);
  };

  const onGenerateSubmitHandler = (teamNumber: number) => {
    generateTournament(teamNumber)
      .then(() => {
        const successMessage = `Successfully generated a tournament with ${teamNumber} teams`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
      })
      .catch((e) => {
        console.log(e);
        dispatch(
          alertActions.changeAlert({
            type: "error",
            message: errorMessageFromAxiosError(e),
          })
        );
      })
      .finally(() => setModalStatus(Modal.None));
  };

  return (
    <>
      <Typography variant="h4">Admin panel</Typography>
      <br />
      <Typography variant="subtitle1">
        You can manage the system from here!
      </Typography>
      <br />
      <br />
      <Stack direction="row" spacing={2}>
        <Button
          variant="contained"
          sx={{
            size: { xs: "small", md: "medium" },
            whiteSpace: "nowrap",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
          onClick={() => navigate("/users")}
        >
          Users
        </Button>
        <Button
          variant="contained"
          sx={{
            size: { xs: "small", md: "medium" },
            whiteSpace: "nowrap",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
          onClick={() => setModalStatus(Modal.Generate)}
        >
          Generate tournaments
        </Button>
      </Stack>
      {modalStatus === Modal.Generate && (
        <GenerateTournamentModal
          onSubmit={onGenerateSubmitHandler}
          onClose={closeModals}
        />
      )}
    </>
  );
};

export default AdminPanel;
