import {
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Chip,
  Divider,
  Grid,
  IconButton,
  Tooltip,
  Typography,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { Box } from "@mui/system";
import React from "react";
import { useNavigate } from "react-router-dom";
import { GameStatus, Team, Game } from "../../utils/types";
import DeleteGameModal from "./DeleteGameModal";
import {
  addTeamToGame,
  changeGameSetScore,
  deleteGame,
  getGame,
  joinGame,
  removeTeamFromGame,
  startGame,
} from "../../services/game.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useAppSelector } from "../../utils/hooks";
import GroupAddIcon from "@mui/icons-material/GroupAdd";
import RequestJoinGameModal from "./RequestJoinGameModal";
import { getUserTeams } from "../../services/team.service";
import AcceptTeamModal from "./AcceptTeamModal";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import Alert from "@mui/material/Alert/Alert";
import GameSets from "./GameSets";
import Loader from "../layout/Loader";

interface GameBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Join = 1,
  Accept = 2,
  Delete = 3,
}

const GameBigCard = (props: GameBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [game, setGame] = React.useState<Game>();

  const user = useAppSelector((state) => state.auth.user);
  const [userTeams, setUserTeams] = React.useState<Team[]>([]);

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [requestJoinInput, setRequestJoinInput] = React.useState("");
  const [acceptTeamInput, setAcceptTeamInput] = React.useState("");

  const [requestJoinError, setRequestJoinError] = React.useState("");
  const [startError, setStartError] = React.useState("");
  const [acceptTeamError, setAcceptTeamError] = React.useState("");
  const [deleteTeamError, setDeleteTeamError] = React.useState("");

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!id) {
      return navigate("/", { replace: true });
    } else {
      getGame(id, abortController.signal)
        .then((res) => {
          setError("");
          setGame(res);
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
    setDeleteTeamError("");
    setModalStatus(Modal.None);
  };

  const onRequestJoinGameSubmit = () => {
    joinGame(id, requestJoinInput)
      .then(() => {
        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
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

  const onGameStartSubmit = () => {
    startGame(id)
      .then(() => {

        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
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

  const onAcceptTeamSubmit = () => {
    addTeamToGame(id, acceptTeamInput)
      .then(() => {
        closeModal();

        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
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

  const onRemoveTeamFromGameSubmit = (team: boolean) => {
    removeTeamFromGame(id, team).then(() => {
      closeModal();

      getGame(id)
        .then((res) => {
          setError("");
          setGame(res);
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

  const onDeleteSubmit = () => {
    deleteGame(id)
      .then(() => {
        navigate("/mygames", { replace: true });
      })
      .catch((e) => {
        console.log(e);
        setDeleteTeamError(errorMessageFromAxiosError(e));
      });
  };

  const onChangeScore = (setId: string, playerId: string, change: boolean) => {
    changeGameSetScore(id, setId, playerId, change)
      .then(() => {
        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
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
      });
  };

  let statusString = "";
  switch (game?.status) {
    case GameStatus.New:
      statusString = "New";
      break;
    case GameStatus.SingleTeam:
      statusString = "Single team";
      break;
    case GameStatus.Ready:
      statusString = "Ready to start";
      break;
    case GameStatus.Started:
      statusString = "In progress";
      break;
    case GameStatus.Finished:
      statusString = "Finished";
      break;
  }

  return (
    <>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Loader isOpen={isLoading} />
      {game && (
        <Card>
          <CardHeader
            title={game.title}
            subheader={<Chip label={statusString} />}
          />
          <CardContent>
            {startError && (
              <>
                <Alert severity="error">{startError}</Alert>
                <br />
              </>
            )}
            <Grid
              container
              spacing={0}
              direction="row"
              alignItems="center"
              justifyContent="space-between"
            >
              <Grid item>
                {game.firstTeam && (
                  <>
                    <Typography
                      variant="body1"
                      color={
                        game.winner
                          ? game.winner.id === game.firstTeam.id
                            ? "green"
                            : "red"
                          : "default"
                      }
                    >
                      {game.firstTeam.title}
                    </Typography>
                    {user?.id === game.ownerId &&
                      game.status < GameStatus.Started && (
                        <IconButton
                          centerRipple={false}
                          color="error"
                          onClick={() => onRemoveTeamFromGameSubmit(false)}
                          size="small"
                        >
                          <Tooltip title="Remove team">
                            <DeleteForeverIcon />
                          </Tooltip>
                        </IconButton>
                      )}
                  </>
                )}
              </Grid>
              <Grid item>
                {game.secondTeam && (
                  <>
                    {user?.id === game.ownerId &&
                      game.status < GameStatus.Started && (
                        <IconButton
                          centerRipple={false}
                          color="error"
                          onClick={() => onRemoveTeamFromGameSubmit(true)}
                          size="small"
                        >
                          <Tooltip title="Remove team">
                            <DeleteForeverIcon />
                          </Tooltip>
                        </IconButton>
                      )}
                    <Typography
                      variant="body1"
                      color={
                        game.winner
                          ? game.winner.id === game.secondTeam.id
                            ? "green"
                            : "red"
                          : "default"
                      }
                    >
                      {game.secondTeam.title}
                    </Typography>
                  </>
                )}
              </Grid>
            </Grid>
            <Grid
              container
              spacing={0}
              direction="row"
              alignItems="center"
              justifyContent="space-between"
            >
              <Grid item>
                {game.status >= GameStatus.Started && (
                  <Typography
                    variant="h3"
                    display="inline"
                    width="100%"
                    textAlign="left"
                    color={
                      game.winner
                        ? game.winner.id === game.firstTeam.id
                          ? "green"
                          : "red"
                        : "default"
                    }
                  >
                    {game.firstTeamScore}
                  </Typography>
                )}
              </Grid>
              <Grid item>
                {game.status >= GameStatus.Started && (
                  <Typography
                    variant="h3"
                    display="inline"
                    width="100%"
                    textAlign="right"
                    color={
                      game.winner
                        ? game.winner.id === game.secondTeam.id
                          ? "green"
                          : "red"
                        : "default"
                    }
                  >
                    {game.secondTeamScore}
                  </Typography>
                )}
              </Grid>
            </Grid>
            <Typography variant="body1">{game.description}</Typography>
            <Typography variant="body2" color="text.secondary">
              Created at: {game.createDate}
            </Typography>
            {user?.id === game.ownerId && (
              <Typography variant="body2" color="text.secondary">
                Last edited at: {game.lastEditDate}
              </Typography>
            )}
            <Divider />
            <GameSets
              isOwner={user?.id === game.ownerId}
              sets={game.sets}
              onChangeScore={onChangeScore}
            />
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              {user &&
                userTeams &&
                game.status != GameStatus.Started &&
                game.status != GameStatus.Finished && (
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
            {user?.id === game.ownerId && game.status === GameStatus.Ready && (
              <IconButton
                centerRipple={false}
                color="success"
                onClick={onGameStartSubmit}
              >
                <Tooltip title="Start game">
                  <PlayCircleOutlineIcon />
                </Tooltip>
              </IconButton>
            )}
            {user?.id === game.ownerId && game.status < GameStatus.Started && (
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
            {user?.id === game.ownerId && game.status < GameStatus.Finished && (
              <IconButton
                centerRipple={false}
                onClick={() => navigate(`/editgame/${id}`)}
              >
                <Tooltip title="Edit game">
                  <EditIcon />
                </Tooltip>
              </IconButton>
            )}
            {user?.id === game.ownerId && (
              <IconButton
                centerRipple={false}
                color="error"
                onClick={() => setModalStatus(Modal.Delete)}
              >
                <Tooltip title="Remove game">
                  <DeleteForeverIcon />
                </Tooltip>
              </IconButton>
            )}
          </CardActions>
        </Card>
      )}
      {modalStatus === Modal.Join && (
        <RequestJoinGameModal
          errorMessage={requestJoinError}
          teams={
            game?.requestedTeams
              ? userTeams.filter(
                  (x) => !game?.requestedTeams.some((y) => y.id == x.id)
                )
              : userTeams
          }
          joinTeamInput={requestJoinInput}
          onJoinGameInputChange={setRequestJoinInput}
          onSubmit={onRequestJoinGameSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Accept && (
        <AcceptTeamModal
          errorMessage={acceptTeamError}
          teams={game?.requestedTeams ?? []}
          acceptTeamInput={acceptTeamInput}
          onAcceptTeamInputChange={setAcceptTeamInput}
          onSubmit={onAcceptTeamSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Delete && (
        <DeleteGameModal
          errorMessage={deleteTeamError}
          onSubmit={onDeleteSubmit}
          onClose={closeModal}
        />
      )}
    </>
  );
};

export default GameBigCard;
