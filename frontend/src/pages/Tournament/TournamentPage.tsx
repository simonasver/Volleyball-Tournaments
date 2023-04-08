import { Grid } from "@mui/material";
import React from "react";
import { useParams, useNavigate } from "react-router-dom";
import BackButton from "../../components/layout/BackButton";
import Layout from "../../components/layout/Layout";
import TournamentBigCard from "../../components/tournament/TournamentBigCard";

const TournamentPage = () => {
    const { tournamentId } = useParams();
    const navigate = useNavigate();
  
    React.useEffect(() => {
      if (!tournamentId) {
        navigate("/", { replace: true });
      }
    });

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <Grid item sx={{ width: { xs: "100%", md: "70%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="flex-start"
          >
            <Grid item>
              <BackButton title="All tournaments" address="/tournaments" />
            </Grid>
          </Grid>
          <br />
          {tournamentId && <TournamentBigCard id={tournamentId} />}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default TournamentPage;
