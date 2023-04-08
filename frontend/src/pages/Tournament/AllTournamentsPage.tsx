import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import { Button, Grid } from "@mui/material";
import BackButton from "../../components/layout/BackButton";
import TournamentList from "../../components/tournament/TournamentList";

const AllTournamentsPage = () => {
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
        <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="space-between"
          >
            <Grid item>
              <BackButton address="/" />
            </Grid>
            <Grid item>
              <Button
                variant="contained"
                onClick={() => {
                  navigate("/mytournaments");
                }}
              >
                My tournaments
              </Button>
            </Grid>
          </Grid>
        </Grid>
        <br />
        <TournamentList all />
      </Grid>
    </Layout>
  );
};

export default AllTournamentsPage;
