import React from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import { Button, Grid, Typography } from "@mui/material";
import BackButton from "../../components/layout/BackButton";
import TournamentList from "../../components/tournament/TournamentList";
import SearchFilterInput from "../../components/layout/SearchFilterInput";

const AllTournamentsPage = () => {
  const navigate = useNavigate();

  const [searchInput, setSearchInput] = React.useState("");

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
          <Typography variant="h4">All tournaments</Typography>
          <br />
          <br />
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
              <SearchFilterInput
                label="Search by title"
                searchInput={searchInput}
                onSearchInputChange={setSearchInput}
              />
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
        <TournamentList searchInput={searchInput} all />
      </Grid>
    </Layout>
  );
};

export default AllTournamentsPage;
