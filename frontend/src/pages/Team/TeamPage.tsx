import React from "react";
import { Alert, Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import TeamBigCard from "../../components/team/TeamBigCard";
import BackButton from "../../components/layout/BackButton";
import { getTeam } from "../../services/team.service";
import { Team } from "../../utils/types";

const MyTeamsPage = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [team, setTeam] = React.useState<Team>();

  React.useEffect(() => {
    if (!teamId) {
      return navigate("/", { replace: true });
    }

    getTeam(teamId)
      .then((res) => {
        setError("");
        setTeam(res);
        console.log(res);
      })
      .catch((e) => {
        console.log(e);
        if (e.response) {
          setError(e.response.data || "Error");
        } else if (e.request) {
          setError("Connection error");
        } else {
          setError("Error");
        }
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
            justifyContent="flex-start"
          >
            <Grid item>
              <BackButton />
            </Grid>
          </Grid>
          <br />
          {error && (
            <>
              <Alert severity="error">{error}</Alert>
              <br />
            </>
          )}
          {team && (
            <TeamBigCard
              id={team.id}
              title={team.title}
              imageUrl={team.pictureUrl}
              description={team.description}
              createDate={new Date(team.creationDate).toDateString()}
              players={team.players}
            />
          )}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default MyTeamsPage;
