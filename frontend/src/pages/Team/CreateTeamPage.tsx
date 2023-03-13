import { Grid } from "@mui/material";
import Layout from "../../components/layout/Layout";
import CreateTeamForm from "../../components/team/CreateTeamForm";

const CreateTeamPage = () => {
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
