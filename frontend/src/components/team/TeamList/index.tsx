import React from "react";
import { useNavigate } from "react-router-dom";
import { Team } from "../../../utils/types";
import { useAppSelector } from "../../../utils/hooks";
import { getTeams, getUserTeams } from "../../../services/team.service";
import { errorMessageFromAxiosError, isAdmin } from "../../../utils/helpers";
import { Alert, Pagination, Typography } from "@mui/material";
import Loader from "../../layout/Loader";
import TeamSmallCard from "./TeamSmallCard";

interface TeamListProps {
  all?: boolean;
}

const TeamList = (props: TeamListProps) => {
  const { all } = props;

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
      if (all) {
        if (!isAdmin(user)) {
          return navigate("/", { replace: true });
        } else {
          getTeams(abortController.signal)
            .then((res) => {
              setError("");
              setTeams(res);
              setIsLoading(false);
            })
            .catch((e) => {
              console.log(e);
              const errorMessage = errorMessageFromAxiosError(e);
              setError(errorMessage);
              if (errorMessage) {
                setIsLoading(false);
              }
            });
        }
      } else {
        getUserTeams(user.id, abortController.signal)
          .then((res) => {
            setError("");

            setTeams(res);
            setIsLoading(false);
          })
          .catch((e) => {
            console.log(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
            if (errorMessage) {
              setIsLoading(false);
            }
          });
      }
    }
    return () => abortController.abort();
  }, []);

  return (
    <>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
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
          <br />
          <br />
          <br />
          You have no teams yet. Create one!
        </Typography>
      )}
      <Pagination count={10} color="primary"/>
    </>
  );
};

export default TeamList;
