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
  Tooltip,
  Typography,
} from "@mui/material";
import React, { SyntheticEvent } from "react";
import Loader from "../../layout/Loader";
import {
  GameStatus,
  Team,
  Tournament,
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
import { errorMessageFromAxiosError } from "../../../utils/helpers";
import {
  addTeamToTournament,
  getTournament,
  joinTournament,
  removeTeamFromTournament,
  startTournament,
} from "../../../services/tournament.service";
import { getUserTeams } from "../../../services/team.service";
import { alertActions } from "../../../store/alert-slice";
import RemoveTournamentTeamModal from "./RemoveTournamentTeamModal";
import TournamentBracket from "./TournamentBracket";

import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

interface TournamentBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Join = 1,
  Accept = 2,
  Remove = 3,
  Delete = 4,
}

const TournamentBigCard = (props: TournamentBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const [tournament, setTournament] = React.useState<Tournament>();

  const user = useAppSelector((state) => state.auth.user);
  const [userTeams, setUserTeams] = React.useState<Team[]>([]);

  const [requestJoinInput, setRequestJoinInput] = React.useState("");
  const [acceptTeamInput, setAcceptTeamInput] = React.useState("");
  const [removeTeamInput, setRemoveTeamInput] = React.useState("");

  const [requestJoinError, setRequestJoinError] = React.useState("");
  const [acceptTeamError, setAcceptTeamError] = React.useState("");
  const [removeTeamError, setRemoveTeamError] = React.useState("");
  const [startError, setStartError] = React.useState("");

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [teamsExpand, setTeamsExpand] = React.useState(true);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!id) {
      return navigate("/", { replace: true });
    } else {
      getTournament(id, abortController.signal)
        .then((res) => {
          console.log(res);
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
    }
    return () => abortController.abort();
  }, []);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (user) {
      getUserTeams(user?.id, abortController.signal).then((res) => {
        setUserTeams(res);
      });
    }
    return () => abortController.abort();
  }, []);

  const closeModal = () => {
    setRequestJoinError("");
    setRequestJoinInput("");
    setAcceptTeamError("");
    setAcceptTeamInput("");
    setRemoveTeamError("");
    setRemoveTeamInput("");
    setModalStatus(Modal.None);
  };

  const onRequestJoinTournamentSubmit = () => {
    joinTournament(id, requestJoinInput)
      .then(() => {
        closeModal();
        const successMessage = `Requested to join tournament ${
          tournament?.title
        } with team ${userTeams.find((x) => x.id === requestJoinInput)?.title}`;
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
    addTeamToTournament(id, acceptTeamInput)
      .then(() => {
        closeModal();
        const successMessage = `Team ${
          tournament?.requestedTeams?.find((x) => x.id === acceptTeamInput)
            ?.title ?? ""
        } was added to tournament ${tournament?.title}`;
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
        setAcceptTeamError(errorMessageFromAxiosError(e));
      });
  };

  const onRemoveTeamFromTournamentSubmit = () => {
    removeTeamFromTournament(id, removeTeamInput).then(() => {
      closeModal();
      const successMessage = `Team ${
        tournament?.acceptedTeams?.find((x) => x.id === removeTeamInput)
          ?.title ?? ""
      } was removed from the tournament`;
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
            subheader={<><Chip color="primary" variant="outlined" label="Tournament"/><Chip label={TournamentStatus[tournament.status]} /></>}
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
            {tournament.winner && <Typography variant="h6" color="success">Tournament winner is {tournament.winner.title}!</Typography>}
            <Typography variant="body1">{tournament.description}</Typography>
            <Typography variant="body2" color="text.secondary">
              Created at: {new Date(tournament.createDate).toLocaleString()}
            </Typography>
            {user?.id === tournament.ownerId && (
              <Typography variant="body2" color="text.secondary">
                Last edited at: {new Date(tournament.lastEditDate).toLocaleString()}
              </Typography>
            )}
            <Divider />
            <br />
            <Accordion expanded={teamsExpand} onChange={(event: SyntheticEvent<Element, Event>, expanded: boolean) => setTeamsExpand(expanded)} variant="outlined">
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography sx={{ width: "33%", flexShrink: 0 }}>
                  Teams
                </Typography>
                <Typography sx={{ color: "text.secondary" }}>
                  Tournament team list
                </Typography>
              </AccordionSummary>
              <AccordionDetails>
                {tournament.acceptedTeams.map((item, index) => <Typography key={item.id}>{index+1}. {item.title}</Typography>)}
                {(!tournament.acceptedTeams || tournament.acceptedTeams.length === 0) && <Typography>No teams in this tournament yet!</Typography>}
              </AccordionDetails>
            </Accordion>
            <br />
            {tournament.matches && tournament.matches.length > 0 && <TournamentBracket tournamentGames={tournament.matches}/>}
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              {user?.id === tournament.ownerId &&
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
            {user?.id === tournament.ownerId &&
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
            {user?.id === tournament.ownerId &&
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
            {user?.id === tournament.ownerId &&
              tournament.status < GameStatus.Finished && (
                <IconButton
                  centerRipple={false}
                  onClick={() => navigate(`/edittournament/${id}`)}
                >
                  <Tooltip title="Edit tournament">
                    <EditIcon />
                  </Tooltip>
                </IconButton>
              )}
            {user?.id === tournament.ownerId && (
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
                    !tournament?.acceptedTeams.some((y) => y.id === x.id)
                )
              : userTeams
          }
          joinTeamInput={requestJoinInput}
          onJoinTournamentInputChange={setRequestJoinInput}
          onSubmit={onRequestJoinTournamentSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Accept && (
        <AcceptTeamModal
          errorMessage={acceptTeamError}
          teams={tournament?.requestedTeams ?? []}
          acceptTeamInput={acceptTeamInput}
          onAcceptTeamInputChange={setAcceptTeamInput}
          onSubmit={onAcceptTeamSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Remove && (
        <RemoveTournamentTeamModal
          errorMessage={removeTeamError}
          teams={tournament?.acceptedTeams ?? []}
          removeTeamInput={removeTeamInput}
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
    </>
  );
};

export default TournamentBigCard;
