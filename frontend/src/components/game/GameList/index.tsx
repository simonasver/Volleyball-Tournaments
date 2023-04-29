import React from "react";
import { useAppSelector } from "../../../utils/hooks";
import { useNavigate, useSearchParams } from "react-router-dom";
import { getGames, getUserGames } from "../../../services/game.service";
import { errorMessageFromAxiosError, formatPaginationDataToQuery, getDefaultPageSize, getDefaultPaginationData } from "../../../utils/helpers";
import { Alert, Pagination, Typography } from "@mui/material";
import Loader from "../../layout/Loader";
import GameSmallCard from "./GameSmallCard";
import { Game, PageData } from "../../../utils/types";

interface GameListProps {
  all?: boolean;
}

const GameList = (props: GameListProps) => {
  const { all } = props;

  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [games, setGames] = React.useState<Game[]>();

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
      navigate(`/${all ? "games" : "mygames"}?${formatPaginationDataToQuery(getDefaultPaginationData())}`);
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
        getUserGames(user.id, currentPage?.pageNumber, currentPage?.pageSize, abortController.signal)
          .then((res) => {
            setGames(res.data);
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
      getGames(currentPage?.pageNumber, currentPage?.pageSize, abortController.signal)
        .then((res) => {
          setGames(res.data);
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
        games &&
        games.map((item) => (
          <>
            <GameSmallCard
              key={item.id}
              id={item.id}
              title={item.title}
              pictureUrl={item.pictureUrl}
              description={item.description}
              createDate={new Date(item.createDate).toLocaleString()}
              status={item.status}
              onButtonPress={() => navigate("/game/" + item.id)}
            />
            <br />
          </>
        ))}
      {!isLoading && (!games || (games && games.length === 0)) && (
        <Typography variant="h6">
          <br />
          <br />
          <br />
          There are no games yet. Create one!
        </Typography>
      )}
      {pagination && <Pagination defaultPage={currentPage?.pageNumber} count={pagination.totalPages} onChange={(event: React.ChangeEvent<unknown>, page: number) => setSearchParams(page, currentPage.pageSize)} color="primary"/>}
    </>
  );
};

export default GameList;
