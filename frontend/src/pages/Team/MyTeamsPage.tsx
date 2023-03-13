import { Button, Grid } from "@mui/material";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import CreateTeamForm from "../../components/team/CreateTeamForm";

const MyTeamsPage = () => {
  const navigate = useNavigate();

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <Button variant="contained" onClick={() => { navigate("/createteam"); }}>Create team</Button>
      </Grid>
    </Layout>
  );
};

export default MyTeamsPage;
