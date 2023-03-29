import {
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Grid,
  IconButton,
  Tab,
  Tabs,
  Tooltip,
  Typography,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { Box } from "@mui/system";
import React from "react";
import { useNavigate } from "react-router-dom";
import { GameStatus, Set, Team } from "../../utils/types";
import DeleteGameModal from "./DeleteGameModal";
import {
  addTeamToGame,
  deleteGame,
  joinGame,
  removeTeamFromGame,
  startGame,
} from "../../services/game.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import { useAppSelector } from "../../utils/hooks";
import GroupAddIcon from "@mui/icons-material/GroupAdd";
import RequestJoinGameModal from "./RequestJoinGameModal";
import { getUserTeams } from "../../services/team.service";
import AcceptTeamModal from "./AcceptTeamModal copy";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import Alert from "@mui/material/Alert/Alert";

interface GameBigCardProps {
  id: string;
  ownerId: string;
  title: string;
  description: string;
  createDate: string;
  status: GameStatus;
  firstTeam: Team;
  secondTeam: Team;
  firstTeamScore: number;
  secondTeamScore: number;
  requestedTeams: Team[];
  sets: Set[];
}

enum Modal {
  None = 0,
  Join = 1,
  Accept = 2,
  Delete = 3,
}

const GameBigCard = (props: GameBigCardProps) => {
  const {
    id,
    ownerId,
    title,
    description,
    createDate,
    status,
    firstTeam,
    secondTeam,
    firstTeamScore,
    secondTeamScore,
    requestedTeams,
    sets,
  } = props;

  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);
  const [userTeams, setUserTeams] = React.useState<Team[]>([]);

  const [modalStatus, setModalStatus] = React.useState(Modal.None);
  const [selectedSet, setSelectedSet] = React.useState(0);

  const [requestJoinInput, setRequestJoinInput] = React.useState("");
  const [acceptTeamInput, setAcceptTeamInput] = React.useState("");

  const [requestJoinError, setRequestJoinError] = React.useState("");
  const [startError, setStartError] = React.useState("");
  const [acceptTeamError, setAcceptTeamError] = React.useState("");
  const [deleteTeamError, setDeleteTeamError] = React.useState("");

  React.useEffect(() => {
    if (user) {
      getUserTeams(user?.id).then((res) => {
        setUserTeams(res);
      });
    }
  }, []);

  const closeModal = () => {
    setRequestJoinError("");
    setRequestJoinInput("");
    setDeleteTeamError("");
    setModalStatus(Modal.None);
  };

  const onSetChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedSet(newValue);
  };

  const onRequestJoinGameSubmit = () => {
    joinGame(id, requestJoinInput)
      .then(() => navigate(0))
      .catch((e) => {
        console.log(e);
        setRequestJoinError(errorMessageFromAxiosError(e));
      });
  };

  const onGameStartSubmit = () => {
    startGame(id)
      .then(() => navigate(0))
      .catch((e) => {
        console.log(e);
        setStartError(errorMessageFromAxiosError(e));
      });
  };

  const onAcceptTeamSubmit = () => {
    addTeamToGame(id, acceptTeamInput)
      .then(() => navigate(0))
      .catch((e) => {
        console.log(e);
        setAcceptTeamError(errorMessageFromAxiosError(e));
      });
  };

  const onRemoveTeamFromGameSubmit = (team: boolean) => {
    removeTeamFromGame(id, team).then(() => {
      navigate(0);
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

  let statusString = "";
  switch (status) {
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
      <Card>
        <CardHeader title={title} subheader={statusString} />
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
              {firstTeam && (
                <>
                  <Typography variant="caption">{firstTeam.title}</Typography>
                  {user?.id === ownerId && status < GameStatus.Started && (
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
                  <br />
                </>
              )}
            </Grid>
            <Grid item>
              {secondTeam && (
                <>
                  {user?.id === ownerId && status < GameStatus.Started && (
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
                  <Typography variant="caption">{secondTeam.title}</Typography>
                  <br />
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
              {status === 3 && (
                <Typography
                  variant="h3"
                  display="inline"
                  width="100%"
                  textAlign="left"
                >
                  {firstTeamScore}
                </Typography>
              )}
            </Grid>
            <Grid item>
              {status === 3 && (
                <Typography
                  variant="h3"
                  display="inline"
                  width="100%"
                  textAlign="right"
                >
                  {secondTeamScore}
                </Typography>
              )}
            </Grid>
          </Grid>
          <Typography variant="body1">{description}</Typography>
          <Typography variant="body2" color="text.secondary">
            Created at: {createDate}
          </Typography>
          <Tabs value={selectedSet} onChange={onSetChange} centered>
            {sets.map((set, index) => {
              return <Tab key={set.id} label={`Set ${index + 1}`} />;
            })}
          </Tabs>
        </CardContent>
        <CardActions>
          <Box sx={{ flexGrow: 1 }}>
            {user &&
              userTeams &&
              status != GameStatus.Started &&
              status != GameStatus.Finished && (
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
          {user?.id === ownerId && status === GameStatus.Ready && (
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
          {user?.id === ownerId && (
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
          {user?.id === ownerId && (
            <IconButton
              centerRipple={false}
              onClick={() => navigate(`/editgame/${id}`)}
            >
              <Tooltip title="Edit game">
                <EditIcon />
              </Tooltip>
            </IconButton>
          )}
          {user?.id === ownerId && (
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
      {modalStatus === Modal.Join && (
        <RequestJoinGameModal
          errorMessage={requestJoinError}
          teams={
            requestedTeams
              ? userTeams.filter(
                  (x) => !requestedTeams.some((y) => y.id == x.id)
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
          teams={requestedTeams}
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
