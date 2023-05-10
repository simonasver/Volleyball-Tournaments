import React from "react";
import { Button, Grid, Typography } from "@mui/material";
import Layout from "../../components/layout/Layout";
import BackButton from "../../components/layout/BackButton";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import TournamentList from "../../components/tournament/TournamentList";
import SearchFilterInput from "../../components/layout/SearchFilterInput";

const MyTournamentsPage = () => {
  const navigate = useNavigate();

  const [searchInput, setSearchInput] = React.useState("");

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
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
        <Grid item sx={{ width: { xs: "100%", md: "70%" } }}>
          <Typography variant="h4">My tournaments</Typography>
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
              <BackButton address="/tournaments" title="All tournaments" />
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
                  navigate("/createtournament");
                }}
              >
                Create tournament
              </Button>
            </Grid>
          </Grid>
        </Grid>
        <br />
        <TournamentList searchInput={searchInput} />
      </Grid>
    </Layout>
  );
};

export default MyTournamentsPage;
