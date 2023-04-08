import { Grid } from "@mui/material";
import Layout from "../../components/layout/Layout";
import CreateTeamForm from "../../components/team/CreateTeamForm";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import React from "react";

const CreateTeamPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if(!user) {
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
        <CreateTeamForm />
      </Grid>
    </Layout>
  );
};

export default CreateTeamPage;
