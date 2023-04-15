import { useNavigate } from "react-router-dom";
import Layout from "../components/layout/Layout";
import { useAppDispatch, useAppSelector } from "../utils/hooks";
import { errorMessageFromAxiosError, isAdmin } from "../utils/helpers";
import { Button, TextField } from "@mui/material";
import React from "react";
import { generateTournament } from "../services/admin.service";
import { alertActions } from "../store/alert-slice";

const AdminPanel = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const user = useAppSelector((state) => state.auth.user);

  const [teamNumber, setTeamNumber] = React.useState<number>(2);

  React.useEffect(() => {
    if (!user || !isAdmin(user)) {
      return navigate("/", { replace: true });
    }
  }, []);

  const onGenerateSubmitHandler = (event: React.FormEvent) => {
    event.preventDefault();
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
      });
  };

  return (
    <Layout>
      <form onSubmit={onGenerateSubmitHandler}>
        <TextField
          label="Team number"
          variant="outlined"
          value={teamNumber}
          type="number"
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setTeamNumber(parseInt(e.target.value ?? 0))
          }
        />
        <Button type="submit">Generate</Button>
      </form>
    </Layout>
  );
};

export default AdminPanel;
