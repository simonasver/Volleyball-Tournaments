import { Grid } from "@mui/material";
import EditGameForm from "../../components/game/EditGameForm";
import Layout from "../../components/layout/Layout";

const EditGamePage = () => {
  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <EditGameForm />
      </Grid>
    </Layout>
  );
};

export default EditGamePage;
