import { Grid } from "@mui/material";
import Layout from "../../components/layout/Layout";
import EditTeamForm from "../../components/team/EditTeamForm";

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
        <EditTeamForm />
      </Grid>
    </Layout>
  );
};

export default CreateTeamPage;
