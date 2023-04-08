import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import React from "react";
import { Grid } from "@mui/material";
import Layout from "../../components/layout/Layout";
import EditTournamentForm from "../../components/tournament/EditTournamentForm";

const EditTournamentPage = () => {
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
        <EditTournamentForm />
      </Grid>
    </Layout>
  );
};

export default EditTournamentPage;
