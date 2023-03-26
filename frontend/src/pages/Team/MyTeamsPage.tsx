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
import Loader from "../../components/layout/Loader";

const MyTeamsPage = () => {
  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [teams, setTeams] = React.useState<Team[]>();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      getUserTeams(user.id, abortController.signal)
        .then((res) => {
          setError("");
          console.log(res);
          setTeams(res);
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
            justifyContent="space-between"
          >
            <Grid item>
              <BackButton address="/" />
            </Grid>
            {isAdmin(user) && (
              <Grid item>
                <Button variant="outlined" onClick={() => navigate("/teams")}>All teams</Button>
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
        <Loader isOpen={isLoading} />
        {!isLoading &&
          teams &&
          teams.map((item) => (
            <>
              <TeamSmallCard
                key={item.id}
                title={item.title}
                imageUrl={item.pictureUrl}
                description={item.description}
                onButtonPress={() => navigate(`/team/${item.id}`)}
                createDate={new Date(item.createDate).toDateString()}
              />
              <br />
            </>
          ))}
        {!isLoading && (!teams || (teams && teams.length === 0)) && (
          <Typography variant="h6">
            You have no teams yet. Create one!
          </Typography>
        )}
      </Grid>
    </Layout>
  );
};

export default MyTeamsPage;
