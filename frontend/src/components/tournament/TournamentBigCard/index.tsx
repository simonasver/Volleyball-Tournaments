import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Box,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Chip,
  Divider,
  IconButton,
  Tab,
  Tabs,
  Tooltip,
  Typography,
} from "@mui/material";
import React, { SyntheticEvent } from "react";
import Loader from "../../layout/Loader";
import {
  GameTeam,
  Team,
  Tournament,
  TournamentMatch,
  TournamentStatus,
} from "../../../utils/types";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";

import EditIcon from "@mui/icons-material/Edit";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import GroupAddIcon from "@mui/icons-material/GroupAdd";
import GroupRemoveIcon from "@mui/icons-material/GroupRemove";
import RequestJoinTournamentModal from "./RequestJoinTournamentModal";
import AcceptTeamModal from "./AcceptTournamentTeamModal";
import DeleteTournamentModal from "./DeleteTournamentModal";
import {
  errorMessageFromAxiosError,
  isManager,
  isOwner,
} from "../../../utils/helpers";
import {
  addTeamToTournament,
  addTournamentManager,
  getTournament,
  getTournamentMatches,
  joinTournament,
  removeTeamFromTournament,
  removeTournamentManager,
  startTournament,
} from "../../../services/tournament.service";
import { getUserTeams } from "../../../services/team.service";
import { alertActions } from "../../../store/alert-slice";
import RemoveTournamentTeamModal from "./RemoveTournamentTeamModal";
import TournamentBracket from "./TournamentBracket";

import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import TournamentSettings from "./TournamentSettings";
import { User } from "../../../store/auth-slice";

import AddManagerModal from "../../shared/ManagerModals/AddManagerModal";
import RemoveManagerModal from "../../shared/ManagerModals/RemoveManagerModal";
import PersonAddAltOutlinedIcon from "@mui/icons-material/PersonAddAltOutlined";
import PersonRemoveAlt1OutlinedIcon from "@mui/icons-material/PersonRemoveAlt1Outlined";
import { getUsers } from "../../../services/user.service";

interface TournamentBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Join = 1,
  Accept = 2,
  Remove = 3,
  Delete = 4,
  AddManager = 5,
  RemoveManager = 6,
}

const TournamentBigCard = (props: TournamentBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [users, setUsers] = React.useState<User[]>();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [currentTab, setCurrentTab] = React.useState(0);
  const onTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue);
  };

  const [tournament, setTournament] = React.useState<Tournament>();
  const [tournamentMatches, setTournamentMatches] =
    React.useState<TournamentMatch[]>();

  const user = useAppSelector((state) => state.auth.user);
  const [userTeams, setUserTeams] = React.useState<Team[]>([]);

  const [requestJoinInput, setRequestJoinInput] = React.useState<Team>();
  const [acceptTeamInput, setAcceptTeamInput] = React.useState<Team>();
  const [removeTeamInput, setRemoveTeamInput] = React.useState<GameTeam>();

  const [requestJoinError, setRequestJoinError] = React.useState("");
  const [acceptTeamError, setAcceptTeamError] = React.useState("");
  const [removeTeamError, setRemoveTeamError] = React.useState("");
  const [startError, setStartError] = React.useState("");

  const [managerSearchInput, setManagerSearchInput] = React.useState("");
  const [addManagerInput, setAddManagerInput] = React.useState<User>();
  const [removeManagerInput, setRemoveManagerInput] = React.useState<User>();
  const [addManagerError, setAddManagerError] = React.useState("");
  const [removeManagerError, setRemoveManagerError] = React.useState("");

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [teamsExpand, setTeamsExpand] = React.useState(true);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!id) {
      return navigate("/", { replace: true });
    } else {
      getTournament(id, abortController.signal)
        .then((res) => {
          setError("");
          setTournament(res);
          getTournamentMatches(id, abortController.signal)
            .then((res) => {
              setTournamentMatches(res);
              setTeamsExpand(!(res && res.length > 0));
              setError("");
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
    return () => abortController.abort();
  }, []);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (user) {
      getUserTeams(user?.id, 1, 99999, "", abortController.signal).then(
        (res) => {
          setUserTeams(res.data);
        }
      );
    }
    return () => abortController.abort();
  }, []);

  React.useEffect(() => {
    const abortController = new AbortController();
    getUsers(1, 20, managerSearchInput, abortController.signal)
      .then((res) => {
        setError("");
        setUsers(res.data);
      })
      .catch((e) => {
        console.log(e);
      });
    return () => abortController.abort();
  }, [managerSearchInput]);

  const closeModal = () => {
    setRequestJoinError("");
    setRequestJoinInput(undefined);
    setAcceptTeamError("");
    setAcceptTeamInput(undefined);
    setRemoveTeamError("");
    setRemoveTeamInput(undefined);
    setAddManagerInput(undefined);
    setAddManagerError("");
    setRemoveManagerInput(undefined);
    setRemoveManagerError("");
    setManagerSearchInput("");
    setModalStatus(Modal.None);
  };

  const onRequestJoinTournamentSubmit = () => {
    joinTournament(id, requestJoinInput?.id ?? "")
      .then(() => {
        closeModal();
        const successMessage = `Requested to join tournament ${tournament?.title} with team ${requestJoinInput?.title}`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getTournament(id)
          .then((res) => {
            setError("");
            setTournament(res);
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
      })
      .catch((e) => {
        console.log(e);
        setRequestJoinError(errorMessageFromAxiosError(e));
      });
  };

  const onAcceptTeamSubmit = () => {
    addTeamToTournament(id, acceptTeamInput?.id ?? "")
      .then(() => {
        closeModal();
        const successMessage = `Team ${
          tournament?.requestedTeams?.find(
            (x) => x.id === acceptTeamInput?.id ?? ""
          )?.title ?? ""
        } was added to tournament ${tournament?.title}`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getTournament(id)
          .then((res) => {
            setError("");
            setTournament(res);
            setIsLoading(false);
            setTeamsExpand(true);
          })
          .catch((e) => {
            console.log(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
            if (errorMessage) {
              setIsLoading(false);
            }
          });
      })
      .catch((e) => {
        console.log(e);
        setAcceptTeamError(errorMessageFromAxiosError(e));
      });
  };

  const onRemoveTeamFromTournamentSubmit = () => {
    removeTeamFromTournament(id, removeTeamInput?.id ?? "").then(() => {
      closeModal();
      const successMessage = `Team ${
        tournament?.acceptedTeams?.find(
          (x) => x.id === removeTeamInput?.id ?? ""
        )?.title ?? ""
      } was removed from the tournament`;
      dispatch(
        alertActions.changeAlert({ type: "success", message: successMessage })
      );

      getTournament(id)
        .then((res) => {
          setError("");
          setTournament(res);
          setIsLoading(false);
          setTeamsExpand(true);
        })
        .catch((e) => {
          console.log(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if (errorMessage) {
            setIsLoading(false);
          }
        });
    });
  };

  const onTournamentStartSubmit = () => {
    startTournament(id)
      .then(() => {
        const successMessage = `Tournament ${tournament?.title} was started`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getTournament(id)
          .then((res) => {
            setError("");
            setTournament(res);
            setIsLoading(false);
            setTeamsExpand(false);

            getTournamentMatches(id)
              .then((res) => {
                setTournamentMatches(res);
                setError("");
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
          })
          .catch((e) => {
            console.log(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
            if (errorMessage) {
              setIsLoading(false);
            }
          });
      })
      .catch((e) => {
        console.log(e);
        setStartError(errorMessageFromAxiosError(e));
      });
  };

  const onAddManagerSubmit = () => {
    if (!addManagerInput) {
      return setAddManagerError("Select cannot be empty");
    }
    addTournamentManager(id, addManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${addManagerInput.userName} was successfully added to ${tournament?.title} tournament managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
      })
      .catch((e) => {
        console.log(e);
        setAddManagerError(errorMessageFromAxiosError(e));
      });
  };

  const onRemoveManagerSubmit = () => {
    if (!removeManagerInput) {
      return setRemoveManagerError("Select cannot be empty");
    }
    removeTournamentManager(id, removeManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${removeManagerInput.userName} was successfully removed from ${tournament?.title} tournament managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
      })
      .catch((e) => {
        console.log(e);
        setRemoveManagerError(errorMessageFromAxiosError(e));
      });
  };

  return (
    <>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Loader isOpen={isLoading} />

      {tournament && (
        <Card>
          <CardHeader
            title={tournament.title}
            subheader={
              <>
                <Chip color="primary" variant="outlined" label="Tournament" />
                {tournament.isPrivate && <Chip label={"Private"} />}
                <Chip label={TournamentStatus[tournament.status]} />
              </>
            }
          />
          {tournament.pictureUrl && (
            <CardMedia
              component="img"
              height="200"
              image={tournament.pictureUrl}
            />
          )}
          <CardContent>
            {startError && (
              <>
                <Alert severity="error">{startError}</Alert>
                <br />
              </>
            )}
            <Tabs
              value={currentTab}
              onChange={onTabChange}
              variant="fullWidth"
              centered
            >
              <Tab value={0} label="Tournament information" />
              <Tab value={1} label="Tournament settings" />
            </Tabs>
            <br />
            <div hidden={currentTab !== 0}>
              {tournament.winner && (
                <>
                  <Typography variant="h5" color="primary" textAlign="center">
                    Tournament winner is {tournament.winner.title}!
                  </Typography>
                  <br />
                </>
              )}
              <Typography variant="body1">{tournament.description}</Typography>
              <Typography variant="body2" color="text.secondary">
                Created at: {new Date(tournament.createDate).toLocaleString()}
              </Typography>
              {isOwner(user, tournament.ownerId) && (
                <Typography variant="body2" color="text.secondary">
                  Last edited at:{" "}
                  {new Date(tournament.lastEditDate).toLocaleString()}
                </Typography>
              )}
              <Divider />
              <br />
              <Accordion
                expanded={teamsExpand}
                onChange={(
                  event: SyntheticEvent<Element, Event>,
                  expanded: boolean
                ) => setTeamsExpand(expanded)}
                variant="outlined"
              >
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Typography sx={{ width: "33%", flexShrink: 0 }}>
                    Teams
                  </Typography>
                  <Typography sx={{ color: "text.secondary" }}>
                    Registered team list
                  </Typography>
                </AccordionSummary>
                <AccordionDetails>
                  {tournament.acceptedTeams
                    .filter((x) => !x.duplicate)
                    .map((item, index) => (
                      <Typography key={item.id}>
                        {index + 1}. {item.title}
                      </Typography>
                    ))}
                  {(!tournament.acceptedTeams ||
                    tournament.acceptedTeams.length === 0) && (
                    <Typography>No teams in this tournament yet!</Typography>
                  )}
                </AccordionDetails>
              </Accordion>
              <br />
              {tournamentMatches && tournamentMatches.length > 0 && (
                <>
                  <Typography variant="body2" textAlign="center">
                    Click on a bracket to check out a game!
                  </Typography>
                  <TournamentBracket tournamentGames={tournamentMatches} />
                </>
              )}
            </div>
            <div hidden={currentTab !== 1}>
              <TournamentSettings
                matchForThirdPlace={tournament.singleThirdPlace}
                teamLimit={tournament.maxTeams}
                basic={tournament.basic}
                pointsToWin={tournament.pointsToWin}
                pointsToWinLastSet={tournament.pointsToWinLastSet}
                pointDifferenceToWin={tournament.pointDifferenceToWin}
                maxSets={tournament.maxSets}
                playersPerTeam={tournament.playersPerTeam}
              />
            </div>
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              {isManager(user, tournament.ownerId, tournament.managers) &&
                tournament.status < TournamentStatus.Started &&
                tournament.acceptedTeams?.length >= 2 && (
                  <IconButton
                    centerRipple={false}
                    color="success"
                    onClick={onTournamentStartSubmit}
                  >
                    <Tooltip title="Start tournament">
                      <PlayCircleOutlineIcon />
                    </Tooltip>
                  </IconButton>
                )}
              {user &&
                userTeams &&
                tournament.status < TournamentStatus.Started && (
                  <IconButton
                    centerRipple={false}
                    onClick={() => setModalStatus(Modal.Join)}
                  >
                    <Tooltip title="Request to join">
                      <GroupAddIcon />
                    </Tooltip>
                  </IconButton>
                )}
            </Box>
            {isManager(user, tournament.ownerId, tournament.managers) &&
              tournament.status < TournamentStatus.Started && (
                <IconButton
                  centerRipple={false}
                  color="success"
                  onClick={() => setModalStatus(Modal.Accept)}
                >
                  <Tooltip title="Add team">
                    <GroupAddIcon />
                  </Tooltip>
                </IconButton>
              )}
            {isManager(user, tournament.ownerId, tournament.managers) &&
              tournament.status < TournamentStatus.Started && (
                <IconButton
                  centerRipple={false}
                  color="error"
                  onClick={() => setModalStatus(Modal.Remove)}
                >
                  <Tooltip title="Remove team">
                    <GroupRemoveIcon />
                  </Tooltip>
                </IconButton>
              )}
            {isManager(user, tournament.ownerId, tournament.managers) &&
              tournament.status < TournamentStatus.Finished && (
                <IconButton
                  centerRipple={false}
                  onClick={() => navigate(`/edittournament/${id}`)}
                >
                  <Tooltip title="Edit tournament">
                    <EditIcon />
                  </Tooltip>
                </IconButton>
              )}
            {isOwner(user, tournament.ownerId) && (
              <IconButton
                centerRipple={false}
                onClick={() => setModalStatus(Modal.AddManager)}
                color="warning"
              >
                <Tooltip title="Add manager">
                  <PersonAddAltOutlinedIcon />
                </Tooltip>
              </IconButton>
            )}
            {isOwner(user, tournament.ownerId) && (
              <IconButton
                centerRipple={false}
                onClick={() => setModalStatus(Modal.RemoveManager)}
                color="warning"
              >
                <Tooltip title="Remove manager">
                  <PersonRemoveAlt1OutlinedIcon />
                </Tooltip>
              </IconButton>
            )}
            {isOwner(user, tournament.ownerId) && (
              <IconButton
                centerRipple={false}
                color="error"
                onClick={() => setModalStatus(Modal.Delete)}
              >
                <Tooltip title="Remove tournament">
                  <DeleteForeverIcon />
                </Tooltip>
              </IconButton>
            )}
          </CardActions>
        </Card>
      )}
      {modalStatus === Modal.Join && (
        <RequestJoinTournamentModal
          errorMessage={requestJoinError}
          teams={
            tournament?.requestedTeams
              ? userTeams.filter(
                  (x) =>
                    !tournament?.requestedTeams.some((y) => y.id === x.id) &&
                    !tournament?.acceptedTeams.some((y) => y.title === x.title)
                )
              : userTeams
          }
          joinTeamInput={requestJoinInput ?? undefined}
          onJoinTournamentInputChange={setRequestJoinInput}
          onSubmit={onRequestJoinTournamentSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Accept && (
        <AcceptTeamModal
          errorMessage={acceptTeamError}
          teams={tournament?.requestedTeams ?? []}
          acceptTeamInput={acceptTeamInput ?? undefined}
          onAcceptTeamInputChange={setAcceptTeamInput}
          onSubmit={onAcceptTeamSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Remove && (
        <RemoveTournamentTeamModal
          errorMessage={removeTeamError}
          teams={tournament?.acceptedTeams ?? []}
          removeTeamInput={removeTeamInput ?? undefined}
          onRemoveTeamInputChange={setRemoveTeamInput}
          onSubmit={onRemoveTeamFromTournamentSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Delete && (
        <DeleteTournamentModal
          tournamentId={id}
          tournamentTitle={tournament?.title ?? ""}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.AddManager && (
        <AddManagerModal
          errorMessage={addManagerError}
          users={users?.filter((x) => x.id !== user?.id && x.id !== tournament?.ownerId) ?? []}
          addManagerInput={addManagerInput}
          onAddManagerInputChange={setAddManagerInput}
          searchInput={managerSearchInput}
          onSearchInputChange={setManagerSearchInput}
          onSubmit={onAddManagerSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.RemoveManager && (
        <RemoveManagerModal
          errorMessage={removeManagerError}
          users={tournament?.managers ?? []}
          removeManagerInput={removeManagerInput}
          onRemoveManagerInputChange={setRemoveManagerInput}
          searchInput={managerSearchInput}
          onSearchInputChange={setManagerSearchInput}
          onSubmit={onRemoveManagerSubmit}
          onClose={closeModal}
        />
      )}
    </>
  );
};

export default TournamentBigCard;
