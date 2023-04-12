import React from "react";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../../utils/hooks";
import { errorMessageFromAxiosError } from "../../../utils/helpers";
import { getTournaments, getUserTournaments } from "../../../services/tournament.service";
import { Tournament } from "../../../utils/types";
import { Alert, Typography } from "@mui/material";
import Loader from "../../layout/Loader";
import TournamentSmallCard from "./TournamentSmallCard";

interface GameListProps {
    all?: boolean;
  }

const TournamentList = (props: GameListProps) => {
    const { all } = props;

  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [tournaments, setTournaments] = React.useState<Tournament[]>();

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!all) {
      if (!user) {
        return navigate("/", { replace: true });
      } else {
        getUserTournaments(user.id, abortController.signal)
          .then((res) => {
            setTournaments(res);
            setIsLoading(false);
          })
          .catch((e) => {
            console.error(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
            if (errorMessage) {
              setIsLoading(false);
            }
          });
      }
    } else {
      getTournaments(abortController.signal)
        .then((res) => {
          setTournaments(res);
          setIsLoading(false);
        })
        .catch((e) => {
          console.error(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if (errorMessage) {
            setIsLoading(false);
          }
        });
    }
    return () => abortController.abort();
  }, []);

    return (<>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Typography variant="h3">{all ? "All tournaments" : "My tournaments"}</Typography>
      <br />
      <Loader isOpen={isLoading} />
      {!isLoading &&
        tournaments &&
        tournaments.map((item) => (
          <>
            <TournamentSmallCard
              key={item.id}
              id={item.id}
              title={item.title}
              pictureUrl={item.pictureUrl}
              description={item.description}
              createDate={new Date(item.createDate).toDateString()}
              status={item.status}
              teamCount={item.acceptedTeams?.length ?? 0}
              maxTeams={item.maxTeams}
              onButtonPress={() => navigate("/tournament/" + item.id)}
            />
            <br />
          </>
        ))}
      {!isLoading && (!tournaments || (tournaments && tournaments.length === 0)) && (
        <Typography variant="h6">
          There are no tournaments yet. Create one!
        </Typography>
      )}
    </>);
};

export default TournamentList;