import React from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { PageData, Team } from "../../../utils/types";
import { useAppSelector } from "../../../utils/hooks";
import { getTeams, getUserTeams } from "../../../services/team.service";
import {
  errorMessageFromAxiosError,
  formatPaginationDataToQuery,
  getDefaultPageSize,
  getDefaultPaginationData,
  isAdmin,
} from "../../../utils/helpers";
import { Alert, Pagination, Typography } from "@mui/material";
import Loader from "../../layout/Loader";
import TeamSmallCard from "./TeamSmallCard";

interface TeamListProps {
  all?: boolean;
}

const TeamList = (props: TeamListProps) => {
  const { all } = props;

  const navigate = useNavigate();

  const [query, setQuery] = useSearchParams();
  const [currentPage, setCurrentPage] = React.useState<{
    pageNumber: number;
    pageSize: number;
  }>({ pageNumber: 1, pageSize: getDefaultPageSize() });

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [teams, setTeams] = React.useState<Team[]>();
  const [pagination, setPagination] = React.useState<PageData>();

  const user = useAppSelector((state) => state.auth.user);

  const setSearchParams = (pageNumber: number, pageSize: number) => {
    query.set("pageNumber", pageNumber.toString());
    query.set("pageSize", pageSize.toString());
    setQuery(query, { replace: true });
    setCurrentPage({ pageNumber, pageSize });
  };

  React.useEffect(() => {
    const pageNumber: number =
      parseInt(query.get("pageNumber") ?? "") ?? getDefaultPageSize();
    const pageSize: number = parseInt(query.get("pageSize") ?? "") ?? 1;
    if (!pageNumber && !pageSize) {
      navigate(
        `/${all ? "teams" : "myteams"}?${formatPaginationDataToQuery(
          getDefaultPaginationData()
        )}`,
        { replace: true }
      );
    } else {
      setSearchParams(pageNumber, pageSize);
    }
  }, [query]);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      if (all) {
        if (!isAdmin(user)) {
          return navigate("/", { replace: true });
        } else {
          getTeams(
            currentPage?.pageNumber,
            currentPage?.pageSize,
            abortController.signal
          )
            .then((res) => {
              setError("");
              setTeams(res.data);
              setPagination(res.pagination);
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
        getUserTeams(
          user.id,
          currentPage?.pageNumber,
          currentPage?.pageSize,
          abortController.signal
        )
          .then((res) => {
            setError("");
            setTeams(res.data);
            setPagination(res.pagination);
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
        teams &&
        teams.map((item) => (
          <>
            <TeamSmallCard
              key={item.id}
              title={item.title}
              imageUrl={item.pictureUrl}
              description={item.description}
              players={item.players?.length ?? 0}
              onButtonPress={() => navigate(`/team/${item.id}`)}
              createDate={new Date(item.createDate).toLocaleString()}
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
      {(teams?.length ?? 0) > 0 && (
        <Pagination
          defaultPage={currentPage.pageNumber}
          count={pagination?.totalPages}
          onChange={(event: React.ChangeEvent<unknown>, page: number) =>
            setSearchParams(page, currentPage.pageSize)
          }
          color="primary"
        />
      )}
    </>
  );
};

export default TeamList;
