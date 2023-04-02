import React from "react";
import { Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import TeamBigCard from "../../components/team/TeamBigCard";
import BackButton from "../../components/layout/BackButton";

const TeamPage = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();

  React.useEffect(() => {
    if(!teamId) {
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
        <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="flex-start"
          >
            <Grid item>
              <BackButton title="My teams" address="/myteams" />
            </Grid>
          </Grid>
          <br />
          {teamId && <TeamBigCard id={teamId} />}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default TeamPage;
