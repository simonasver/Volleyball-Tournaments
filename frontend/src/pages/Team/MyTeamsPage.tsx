import React from "react";
import { Alert, Button, Grid, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import BackButton from "../../components/layout/BackButton";
import Layout from "../../components/layout/Layout";
import TeamSmallCard from "../../components/team/TeamSmallCard";
import { getUserTeams } from "../../services/team.service";
import { useAppSelector } from "../../utils/hooks";
import { errorMessageFromAxiosError, isAdmin } from "../../utils/helpers";
import { Team } from "../../utils/types";

const MyTeamsPage = () => {
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [teams, setTeams] = React.useState<Team[]>();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if(!user) {
      navigate("/", { replace: true });
    }

    getUserTeams(user?.id ?? "")
      .then((res) => {
        setError("");
        setTeams(res);
      })
      .catch((e) => {
        console.log(e);
        setError(errorMessageFromAxiosError(e));
      });
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
            justifyContent="space-between"
          >
            <Grid item>
              <BackButton address="/" />
            </Grid>
            {isAdmin(user) && (
              <Grid item>
                <Button>All teams</Button>
              </Grid>
            )}
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
        <br />
        {error && (
          <>
            <Alert severity="error">{error}</Alert>
            <br />
          </>
        )}
        <Typography variant="h3">My teams</Typography>
        <br />
        {teams &&
          teams.map((item) => (
            <>
              <TeamSmallCard
                key={item.id}
                title={item.title}
                imageUrl={item.pictureUrl}
                description={item.description}
                onButtonPress={() => navigate(`/team/${item.id}`)}
                createDate={new Date(item.creationDate).toDateString()}
              />
              <br />
            </>
          ))}
        {((teams && teams.length === 0) || !teams) && <Typography variant="h6">You have no teams yet. Create one!</Typography>}
      </Grid>
    </Layout>
  );
};

export default MyTeamsPage;
