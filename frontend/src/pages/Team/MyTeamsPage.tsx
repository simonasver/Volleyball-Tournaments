import React from "react";
import { Button, Grid } from "@mui/material";
import Layout from "../../components/layout/Layout";
import TeamList from "../../components/team/TeamList";
import BackButton from "../../components/layout/BackButton";
import { useNavigate } from "react-router-dom";

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
                  navigate("/createteam");
                }}
              >
                Create team
              </Button>
            </Grid>
          </Grid>
        </Grid>
        <br />
        <TeamList />
      </Grid>
    </Layout>
  );
};

export default MyTeamsPage;
