import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Avatar,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Chip,
  Divider,
  Grid,
  IconButton,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Tabs,
  Tooltip,
  Typography,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { Box } from "@mui/system";
import React, { SyntheticEvent } from "react";
import { useNavigate } from "react-router-dom";
import {
  GameStatus,
  Team,
  Game,
  GameSet,
  GameScore,
  Log,
} from "../../../utils/types";
import DeleteGameModal from "./DeleteGameModal";
import {
  addGameManager,
  addTeamToGame,
  changeGameSetScore,
  changeGameSetStats,
  getGame,
  getGameLogs,
  joinGame,
  removeGameManager,
  removeTeamFromGame,
  startGame,
} from "../../../services/game.service";
import {
  errorMessageFromAxiosError,
  isGameFull,
  isManager,
  isOwner,
} from "../../../utils/helpers";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import GroupAddIcon from "@mui/icons-material/GroupAdd";
import RequestJoinGameModal from "./RequestJoinGameModal";
import { getUserTeams } from "../../../services/team.service";
import AcceptGameTeamModal from "./AcceptGameTeamModal";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import Alert from "@mui/material/Alert/Alert";
import GameSets from "./GameSets";
import Loader from "../../layout/Loader";
import { alertActions } from "../../../store/alert-slice";
import BackButton from "../../layout/BackButton";
import ArrowCircleRightIcon from "@mui/icons-material/ArrowCircleRight";
import { moveTeamDown } from "../../../services/tournament.service";
import ChangeScoreModal from "./ChangeScoreModal";

import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import GameSettings from "./GameSettings";
import { User } from "../../../store/auth-slice";

import AddManagerModal from "../../shared/ManagerModals/AddManagerModal";
import RemoveManagerModal from "../../shared/ManagerModals/RemoveManagerModal";
import PersonAddAltOutlinedIcon from "@mui/icons-material/PersonAddAltOutlined";
import PersonRemoveAlt1OutlinedIcon from "@mui/icons-material/PersonRemoveAlt1Outlined";
import { getUsers } from "../../../services/user.service";

interface GameBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Join = 1,
  Accept = 2,
  Delete = 3,
  ChangeScore = 4,
  AddManager = 5,
  RemoveManager = 6,
}

const GameBigCard = (props: GameBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [users, setUsers] = React.useState<User[]>();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [game, setGame] = React.useState<Game>();
  const [sets, setSets] = React.useState<GameSet[]>([]);
  const [logs, setLogs] = React.useState<Log[]>();

  const [logsExpanded, setLogsExpanded] = React.useState(false);

  const user = useAppSelector((state) => state.auth.user);
  const [userTeams, setUserTeams] = React.useState<Team[]>([]);

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [requestJoinInput, setRequestJoinInput] = React.useState<Team>();
  const [acceptTeamInput, setAcceptTeamInput] = React.useState<Team>();
  const [changeScoreInput, setChangeScoreInput] = React.useState<GameScore>(
    GameScore.Score
  );
  const [changeScorePositive, setChangeScorePositive] = React.useState(false);
  const [changeScoreSet, setChangeScoreSet] = React.useState("");
  const [changeScorePlayer, setChangeScorePlayer] = React.useState("");

  const [changeScoreHeader, setChangeScoreHeader] = React.useState("");

  const [requestJoinError, setRequestJoinError] = React.useState("");
  const [startError, setStartError] = React.useState("");
  const [acceptTeamError, setAcceptTeamError] = React.useState("");
  const [changeScoreError, setChangeScoreError] = React.useState("");

  const [currentTab, setCurrentTab] = React.useState<number>(0);

  const [managerSearchInput, setManagerSearchInput] = React.useState("");
  const [addManagerInput, setAddManagerInput] = React.useState<User>();
  const [removeManagerInput, setRemoveManagerInput] = React.useState<User>();
  const [addManagerError, setAddManagerError] = React.useState("");
  const [removeManagerError, setRemoveManagerError] = React.useState("");

  const onTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue);
  };

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!id) {
      return navigate("/", { replace: true });
    } else {
      getGame(id, abortController.signal)
        .then((res) => {
          setError("");
          setGame(res);
          setSets(res.sets);
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
      getGameLogs(id, abortController.signal)
        .then((res) => {
          setLogs(res);
        })
        .catch((e) => {
          console.log(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
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
    setChangeScoreError("");
    setChangeScorePlayer("");
    setChangeScoreSet("");
    setChangeScorePositive(false);
    setChangeScoreHeader("");
    setAddManagerInput(undefined);
    setAddManagerError("");
    setRemoveManagerInput(undefined);
    setRemoveManagerError("");
    setManagerSearchInput("");
    setModalStatus(Modal.None);
  };

  const onRequestJoinGameSubmit = () => {
    joinGame(id, requestJoinInput?.id ?? "")
      .then(() => {
        closeModal();
        const successMessage = `Requested to join game ${
          game?.title
        } with team ${
          userTeams.find((x) => x.id === requestJoinInput?.id ?? "")?.title
        }`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

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
        const successMessage = `Game ${game?.title} was started`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
            setSets(res.sets);
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
    addTeamToGame(id, acceptTeamInput?.id ?? "")
      .then(() => {
        closeModal();
        const successMessage = `Team ${
          game?.requestedTeams?.find((x) => x.id === acceptTeamInput?.id ?? "")
            ?.title ?? ""
        } was added to game ${game?.title}`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

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
      const successMessage = `Team ${
        team ? game?.secondTeam.title : game?.firstTeam.title
      } was removed from the game`;
      dispatch(
        alertActions.changeAlert({ type: "success", message: successMessage })
      );

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

  const onMoveTeamDownSubmit = () => {
    if (!game?.tournamentMatch) {
      return;
    }
    moveTeamDown(game?.tournamentMatch.tournament.id, game?.tournamentMatch.id)
      .then(() => {
        dispatch(
          alertActions.changeAlert({
            type: "success",
            message: "Successfully moved the game down the bracket",
          })
        );
      })
      .catch((e) => {
        console.log(e);
        const errorMessage = errorMessageFromAxiosError(e);
        dispatch(
          alertActions.changeAlert({ type: "error", message: errorMessage })
        );
      });
  };

  const onChangeScore = (
    type: GameScore,
    change: boolean,
    fast?: boolean,
    setId?: string,
    playerId?: string
  ) => {
    if (type !== GameScore.Score) {
      return onChangePlayerStats(type, change, fast, setId, playerId);
    }
    changeGameSetScore(
      id,
      setId ?? changeScoreSet,
      playerId ?? changeScorePlayer,
      change
    )
      .then(() => {
        const successMessage = `Player ${
          game?.sets
            .find((x) => x.id === setId || changeScoreSet)
            ?.players.find((x) => x.id === playerId || changeScorePlayer)?.name
        } score was ${change ? "increased" : "decreased"}`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        if (!fast) {
          closeModal();
        }

        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
            const changedIndex = res.sets.findIndex(
              (set: GameSet) => set.id === setId || changeScoreSet
            );
            if (
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion, @typescript-eslint/no-non-null-asserted-optional-chain
              res.sets[changedIndex].firstTeamScore >= game?.pointsToWin! ||
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion, @typescript-eslint/no-non-null-asserted-optional-chain
              res.sets[changedIndex].secondTeamScore >= game?.pointsToWin!
            ) {
              setSets(res.sets);
            } else {
              setSets((currentSets) => {
                currentSets[changedIndex] = res.sets[changedIndex];
                return currentSets;
              });
            }
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

        getGameLogs(id)
          .then((res) => {
            setLogs(res);
          })
          .catch((e) => {
            console.log(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
          });
      })
      .catch((e) => {
        console.log(e);
        const errorMessage = errorMessageFromAxiosError(e);
        dispatch(
          alertActions.changeAlert({ type: "error", message: errorMessage })
        );
      });
  };

  const onChangePlayerStats = (
    type: GameScore,
    change: boolean,
    fast?: boolean,
    setId?: string,
    playerId?: string
  ) => {
    changeGameSetStats(
      id,
      setId ?? changeScoreSet,
      playerId ?? changeScorePlayer,
      type,
      change
    )
      .then(() => {
        const successMessage = `Player ${
          game?.sets
            .find((x) => x.id === setId || changeScoreSet)
            ?.players.find((x) => x.id === playerId || changeScorePlayer)?.name
        } stat ${type} was ${change ? "increased" : "decreased"}`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        if (!fast) {
          closeModal();
        }

        getGame(id)
          .then((res) => {
            setError("");
            setGame(res);
            const changedIndex = res.sets.findIndex(
              (set: GameSet) => set.id === setId || changeScoreSet
            );
            if (
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion, @typescript-eslint/no-non-null-asserted-optional-chain
              res.sets[changedIndex].firstTeamScore >= game?.pointsToWin! ||
              // eslint-disable-next-line @typescript-eslint/no-non-null-assertion, @typescript-eslint/no-non-null-asserted-optional-chain
              res.sets[changedIndex].secondTeamScore >= game?.pointsToWin!
            ) {
              setSets(res.sets);
            } else {
              setSets((currentSets) => {
                currentSets[changedIndex] = res.sets[changedIndex];
                return currentSets;
              });
            }
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

        getGameLogs(id)
          .then((res) => {
            setLogs(res);
          })
          .catch((e) => {
            console.log(e);
            const errorMessage = errorMessageFromAxiosError(e);
            setError(errorMessage);
          });
      })
      .catch((e) => {
        console.log(e);
        const errorMessage = errorMessageFromAxiosError(e);
        dispatch(
          alertActions.changeAlert({ type: "error", message: errorMessage })
        );
      });
  };

  /**
   *
   * @param setId
   * @param playerId
   * @param change false - decrease, true - increase
   * @param fast true - don't show modal
   */
  const onChangeScoreOpen = (
    setId: string,
    playerId: string,
    change: boolean,
    fast?: boolean
  ) => {
    setChangeScoreSet(setId);
    setChangeScorePlayer(playerId);
    setChangeScorePositive(change);
    if (fast) {
      onChangeScore(changeScoreInput, change, fast, setId, playerId);
    } else {
      const set = game?.sets.find((x) => x.id === setId);
      setChangeScoreHeader(
        `${change ? "Increase" : "Decrease"} ${game?.title} game ${
          set?.players.find((x) => x.id === playerId)?.name
        } player score in set ${set?.number}`
      );
      setModalStatus(Modal.ChangeScore);
    }
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

  const getSubHeader = () => {
    return (
      <>
        <Chip color="primary" variant="outlined" label={<b>Game</b>} />
        {game?.isPrivate && <Chip label="Private" />}
        <Chip label={statusString} />
        {game?.tournamentMatch && (
          <Chip
            label="Tournament game"
            clickable
            onClick={() =>
              navigate(`/tournament/${game?.tournamentMatch.tournament.id}`)
            }
          />
        )}
      </>
    );
  };

  const onAddManagerSubmit = () => {
    if (!addManagerInput) {
      return setAddManagerError("Select cannot be empty");
    }
    addGameManager(id, addManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${addManagerInput.userName} was successfully added to ${game?.title} game managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
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
        setAddManagerError(errorMessageFromAxiosError(e));
      });
  };

  const onRemoveManagerSubmit = () => {
    if (!removeManagerInput) {
      return setRemoveManagerError("Select cannot be empty");
    }
    removeGameManager(id, removeManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${removeManagerInput.userName} was successfully removed from ${game?.title} game managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
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
        setRemoveManagerError(errorMessageFromAxiosError(e));
      });
  };

  return (
    <>
      <Grid
        container
        spacing={1}
        direction="row"
        alignItems="center"
        justifyContent="flex-start"
      >
        <Grid item>
          {game?.tournamentMatch && (
            <BackButton
              title="Tournament"
              address={`/tournament/${game.tournamentMatch.tournament.id}`}
            />
          )}
          {!game?.tournamentMatch && (
            <BackButton title="All games" address="/games" />
          )}
        </Grid>
      </Grid>
      <br />
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Loader isOpen={isLoading} />
      {game && (
        <Card>
          <CardHeader title={game.title} subheader={getSubHeader()} />
          {game.pictureUrl && (
            <CardMedia component="img" height="200" image={game.pictureUrl} />
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
              <Tab value={0} label="Game information" />
              <Tab value={1} label="Game settings" />
            </Tabs>
            <br />
            <div hidden={currentTab !== 0}>
              <Grid
                container
                spacing={0}
                direction="row"
                alignItems="center"
                justifyContent="space-between"
              >
                <Grid item>
                  {game.firstTeam && (
                    <Grid
                      container
                      spacing={0}
                      direction="row"
                      alignItems="center"
                      justifyContent="space-between"
                    >
                      <Grid item>
                        <Typography
                          variant="body1"
                          fontWeight="bold"
                          color={
                            game.winner
                              ? game.winner.id === game.firstTeam.id
                                ? "primary"
                                : "default"
                              : "default"
                          }
                        >
                          <ListItem>
                            <ListItemText primary={game.firstTeam.title} />
                            <ListItemAvatar
                              sx={{ paddingLeft: "16px", minWidth: "0px" }}
                            >
                              <Avatar
                                alt={game.firstTeam.title}
                                src={game.firstTeam.pictureUrl}
                              />
                            </ListItemAvatar>
                          </ListItem>
                        </Typography>
                      </Grid>
                      <Grid item>
                        {isManager(user, game.ownerId, game.managers) &&
                          !game.tournamentMatch &&
                          game.status < GameStatus.Started && (
                            <IconButton
                              centerRipple={false}
                              color="error"
                              onClick={() => onRemoveTeamFromGameSubmit(false)}
                              size="small"
                            >
                              <Tooltip title="Remove first team">
                                <DeleteForeverIcon />
                              </Tooltip>
                            </IconButton>
                          )}
                        {isManager(user, game.ownerId, game.managers) &&
                          game.tournamentMatch &&
                          !game.tournamentMatch.thirdPlace &&
                          game.status != GameStatus.Started &&
                          !isGameFull(game) && (
                            <IconButton
                              centerRipple={false}
                              color="success"
                              onClick={() => onMoveTeamDownSubmit()}
                              size="small"
                            >
                              <Tooltip title="Move team down a bracket">
                                <ArrowCircleRightIcon />
                              </Tooltip>
                            </IconButton>
                          )}
                      </Grid>
                    </Grid>
                  )}
                </Grid>
                <Grid item>
                  {game.secondTeam && (
                    <Grid
                      container
                      spacing={0}
                      direction="row"
                      alignItems="center"
                      justifyContent="space-between"
                    >
                      <Grid item>
                        {isManager(user, game.ownerId, game.managers) &&
                          game.tournamentMatch &&
                          !game.tournamentMatch.thirdPlace &&
                          game.status != GameStatus.Started &&
                          !isGameFull(game) && (
                            <IconButton
                              centerRipple={false}
                              color="success"
                              onClick={() => onMoveTeamDownSubmit()}
                              size="small"
                            >
                              <Tooltip title="Move team down a bracket">
                                <ArrowCircleRightIcon />
                              </Tooltip>
                            </IconButton>
                          )}
                        {isManager(user, game.ownerId, game.managers) &&
                          !game.tournamentMatch &&
                          game.status < GameStatus.Started && (
                            <IconButton
                              centerRipple={false}
                              color="error"
                              onClick={() => onRemoveTeamFromGameSubmit(true)}
                              size="small"
                            >
                              <Tooltip title="Remove second team">
                                <DeleteForeverIcon />
                              </Tooltip>
                            </IconButton>
                          )}
                      </Grid>
                      <Grid item>
                        <Typography
                          variant="body1"
                          fontWeight="bold"
                          color={
                            game.winner
                              ? game.winner.id === game.secondTeam.id
                                ? "primary"
                                : "default"
                              : "default"
                          }
                        >
                          <ListItem>
                            <ListItemAvatar>
                              <Avatar
                                alt={game.secondTeam.title}
                                src={game.secondTeam.pictureUrl}
                              />
                            </ListItemAvatar>
                            <ListItemText primary={game.secondTeam.title} />
                          </ListItem>
                        </Typography>
                      </Grid>
                    </Grid>
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
                            ? "primary"
                            : "default"
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
                            ? "primary"
                            : "default"
                          : "default"
                      }
                    >
                      {game.secondTeamScore}
                    </Typography>
                  )}
                </Grid>
              </Grid>
              <br />
              <Typography variant="body1">{game.description}</Typography>
              <Typography variant="body2" color="text.secondary">
                Created at: {new Date(game.createDate).toLocaleString()}
              </Typography>
              {isManager(user, game.ownerId, game.managers) && (
                <Typography variant="body2" color="text.secondary">
                  Last edited at: {new Date(game.lastEditDate).toLocaleString()}
                </Typography>
              )}
              <Divider />
              <GameSets
                isOwner={isManager(user, game.ownerId, game.managers)}
                sets={sets ?? []}
                basic={game.basic}
                onChangeScore={onChangeScoreOpen}
              />
            </div>
            <div hidden={currentTab !== 1}>
              <GameSettings
                basic={game.basic}
                pointsToWin={game.pointsToWin}
                pointsToWinLastSet={game.pointsToWinLastSet}
                pointDifferenceToWin={game.pointDifferenceToWin}
                maxSets={game.maxSets}
                playersPerTeam={game.playersPerTeam}
              />
            </div>
            <Accordion
              expanded={logsExpanded}
              onChange={(
                event: SyntheticEvent<Element, Event>,
                expanded: boolean
              ) => setLogsExpanded(expanded)}
              variant="outlined"
            >
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography sx={{ width: "33%", flexShrink: 0 }}>
                  Logs
                </Typography>
                <Typography sx={{ color: "text.secondary" }}>
                  Game logs
                </Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Date</TableCell>
                      <TableCell>Message</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {logs?.map((log) => (
                      <TableRow key={log.id}>
                        <TableCell>
                          {new Date(log.time).toLocaleString()}
                        </TableCell>
                        <TableCell>{log.message}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </AccordionDetails>
            </Accordion>
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              {isManager(user, game.ownerId, game.managers) &&
                game.status === GameStatus.Ready && (
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
              {user &&
                userTeams &&
                !game.tournamentMatch &&
                game.status < GameStatus.Ready && (
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
            {isManager(user, game.ownerId, game.managers) &&
              !game.tournamentMatch &&
              game.status < GameStatus.Started && (
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
            {isManager(user, game.ownerId, game.managers) &&
              game.status < GameStatus.Finished && (
                <IconButton
                  centerRipple={false}
                  onClick={() => navigate(`/editgame/${id}`)}
                >
                  <Tooltip title="Edit game">
                    <EditIcon />
                  </Tooltip>
                </IconButton>
              )}
            {isOwner(user, game.ownerId) && (
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
            {isOwner(user, game.ownerId) && (
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
            {isOwner(user, game.ownerId) && !game.tournamentMatch && (
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
                  (x) =>
                    !game?.requestedTeams.some((y) => y.id === x.id) &&
                    x.title !== game?.firstTeam?.title &&
                    x.title !== game?.secondTeam?.title
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
        <AcceptGameTeamModal
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
          gameId={id}
          gameTitle={game?.title ?? ""}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.ChangeScore && (
        <ChangeScoreModal
          errorMessage={changeScoreError}
          basic={game?.basic ?? true}
          header={changeScoreHeader}
          positive={changeScorePositive}
          changeScoreInput={changeScoreInput}
          changeScoreInputChange={setChangeScoreInput}
          onSubmit={onChangeScore}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.AddManager && (
        <AddManagerModal
          errorMessage={addManagerError}
          users={users?.filter((x) => x.id !== game?.ownerId) ?? []}
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
          users={game?.managers ?? []}
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

export default GameBigCard;
