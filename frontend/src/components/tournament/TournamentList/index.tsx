import React from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAppSelector } from "../../../utils/hooks";
import { errorMessageFromAxiosError, formatPaginationDataToQuery, getDefaultPageSize, getDefaultPaginationData } from "../../../utils/helpers";
import { getTournaments, getUserTournaments } from "../../../services/tournament.service";
import { PageData, Tournament } from "../../../utils/types";
import { Alert, Pagination, Typography } from "@mui/material";
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

  const [query, setQuery] = useSearchParams();
  const [currentPage, setCurrentPage] = React.useState<{ pageNumber: number, pageSize: number }>({ pageNumber: 1, pageSize: getDefaultPageSize() });
  const [pagination, setPagination] = React.useState<PageData>();

  const setSearchParams = (pageNumber: number, pageSize: number) => {
    query.set("pageNumber", pageNumber.toString());
    query.set("pageSize", pageSize.toString());
    setQuery(query, { replace: true });
    setCurrentPage({ pageNumber, pageSize });
  };

  React.useEffect(() => {
    const pageNumber: number = parseInt(query.get("pageNumber") ?? "") ?? getDefaultPageSize();
    const pageSize: number = parseInt(query.get("pageSize") ?? "") ?? 1;
    if(!pageNumber && !pageSize) {
      navigate(`/${all ? "tournaments" : "mytournaments"}?${formatPaginationDataToQuery(getDefaultPaginationData())}`);
    } else {
      setSearchParams(pageNumber, pageSize);
    }
  }, [query]);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!all) {
      if (!user) {
        return navigate("/", { replace: true });
      } else {
        getUserTournaments(user.id, currentPage.pageNumber, currentPage.pageSize, abortController.signal)
          .then((res) => {
            setTournaments(res.data);
            setPagination(res.pagination);
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
      getTournaments(currentPage.pageNumber, currentPage.pageSize, abortController.signal)
        .then((res) => {
          setTournaments(res.data);
          setPagination(res.pagination);
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
  }, [currentPage]);

    return (<>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Loader isOpen={isLoading} />
      {!isLoading &&
        tournaments &&
        tournaments.map((item) =>
          <>
            <TournamentSmallCard
              key={item.id}
              id={item.id}
              title={item.title}
              pictureUrl={item.pictureUrl}
              description={item.description}
              createDate={new Date(item.createDate).toLocaleString()}
              status={item.status}
              teamCount={item.acceptedTeams?.filter(x => !x.duplicate).length ?? 0}
              maxTeams={item.maxTeams}
              onButtonPress={() => navigate("/tournament/" + item.id)}
            />
            <br />
          </>
        )}
      {!isLoading && (!tournaments || (tournaments && tournaments.length === 0)) && (
        <Typography variant="h6">
          <br />
          <br />
          <br />
          There are no tournaments yet. Create one!
        </Typography>
      )}
      {pagination && <Pagination defaultPage={currentPage?.pageNumber} count={pagination.totalPages} onChange={(event: React.ChangeEvent<unknown>, page: number) => setSearchParams(page, currentPage.pageSize)} color="primary"/>}
    </>);
};

export default TournamentList;