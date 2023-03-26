import React from "react";
import { Alert, Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import TeamBigCard from "../../components/team/TeamBigCard";
import BackButton from "../../components/layout/BackButton";
import { getTeam } from "../../services/team.service";
import { Team } from "../../utils/types";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import Loader from "../../components/layout/Loader";

const TeamPage = () => {
  const { teamId } = useParams();
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [team, setTeam] = React.useState<Team>();

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!teamId) {
      return navigate("/", { replace: true });
    } else {
      getTeam(teamId, abortController.signal)
        .then((res) => {
          setError("");
          setTeam(res);
          setIsLoading(false);
        })
        .catch((e) => {
          console.log(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if(errorMessage){
            setIsLoading(false);
          }
        })
    }
    return () => abortController.abort();
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
          {error && (
            <>
              <Alert severity="error">{error}</Alert>
              <br />
            </>
          )}
          <Loader isOpen={isLoading}/>
          {!isLoading && team && (
            <TeamBigCard
              id={team.id}
              title={team.title}
              imageUrl={team.pictureUrl}
              description={team.description}
              createDate={new Date(team.createDate).toDateString()}
              players={team.players}
            />
          )}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default TeamPage;
