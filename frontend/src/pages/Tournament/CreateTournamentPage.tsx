import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import React from "react";
import Layout from "../../components/layout/Layout";
import { Grid } from "@mui/material";
import CreateTournamentForm from "../../components/tournament/CreateTournamentForm";

const CreateTournamentPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
      navigate("/", { replace: true });
    }
  }, []);

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <CreateTournamentForm />
      </Grid>
    </Layout>
  );
};

export default CreateTournamentPage;
