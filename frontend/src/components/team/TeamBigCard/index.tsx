import {
  Alert,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Chip,
  IconButton,
  Tooltip,
  Typography,
} from "@mui/material";
import PersonRemoveIcon from "@mui/icons-material/PersonRemove";
import PersonAddAlt1Icon from "@mui/icons-material/PersonAddAlt1";
import EditIcon from "@mui/icons-material/Edit";
import DeleteForeverIcon from "@mui/icons-material/DeleteForever";
import { Box } from "@mui/system";
import React from "react";
import AddPlayerModal from "./AddPlayerModal";
import RemovePlayerModal from "./RemovePlayerModal";
import DeleteTeamModal from "./DeleteTeamModal";
import { useNavigate } from "react-router-dom";
import {
  addPlayerToTeam,
  addTeamManager,
  getTeam,
  removePlayerFromTeam,
  removeTeamManager,
} from "../../../services/team.service";
import { Team } from "../../../utils/types";
import {
  errorMessageFromAxiosError,
  isManager,
  isOwner,
} from "../../../utils/helpers";
import Loader from "../../layout/Loader";
import { alertActions } from "../../../store/alert-slice";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import { User } from "../../../store/auth-slice";
import { getUsers } from "../../../services/user.service";
import AddManagerModal from "../../shared/ManagerModals/AddManagerModal";
import RemoveManagerModal from "../../shared/ManagerModals/RemoveManagerModal";
import PersonAddAltOutlinedIcon from "@mui/icons-material/PersonAddAltOutlined";
import PersonRemoveAlt1OutlinedIcon from "@mui/icons-material/PersonRemoveAlt1Outlined";
import TeamPlayerList from "./TeamPlayerList";

interface TeamBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Add = 1,
  Remove = 2,
  Delete = 3,
  AddManager = 4,
  RemoveManager = 5,
}

const TeamBigCard = (props: TeamBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const user = useAppSelector((state) => state.auth.user);

  const [users, setUsers] = React.useState<User[]>();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [team, setTeam] = React.useState<Team>();

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [addPlayerInput, setAddPlayerInput] = React.useState("");
  const [removePlayerInput, setRemovePlayerInput] = React.useState("");
  const [addManagerInput, setAddManagerInput] = React.useState<User>();
  const [removeManagerInput, setRemoveManagerInput] = React.useState<User>();
  const [managerSearchInput, setManagerSearchInput] = React.useState("");

  const [addPlayerError, setAddPlayerError] = React.useState("");
  const [removePlayerError, setRemovePlayerError] = React.useState("");
  const [addManagerError, setAddManagerError] = React.useState("");
  const [removeManagerError, setRemoveManagerError] = React.useState("");

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!id) {
      return navigate("/", { replace: true });
    } else {
      getTeam(id, abortController.signal)
        .then((res) => {
          setError("");
          setTeam(res);
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
    setAddPlayerInput("");
    setAddPlayerError("");
    setRemovePlayerInput("");
    setRemovePlayerError("");
    setAddManagerInput(undefined);
    setAddManagerError("");
    setRemoveManagerInput(undefined);
    setRemoveManagerError("");
    setManagerSearchInput("");
    setModalStatus(Modal.None);
  };

  const onAddPlayerSubmit = () => {
    if (!addPlayerInput) {
      return setAddPlayerError("Name cannot be empty");
    }
    addPlayerToTeam(id, addPlayerInput)
      .then(() => {
        closeModal();
        const successMessage = `Player ${addPlayerInput} was successfully added to ${team?.title} team`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getTeam(id)
          .then((res) => {
            setError("");
            setTeam(res);
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
        setAddPlayerError(errorMessageFromAxiosError(e));
      });
  };

  const onRemovePlayerSubmit = () => {
    if (!removePlayerInput) {
      return setRemovePlayerError("Select cannot be empty");
    }
    removePlayerFromTeam(id, removePlayerInput)
      .then(() => {
        closeModal();
        const successMessage = `Player ${
          team?.players.find((x) => x.id === removePlayerInput)?.name
        } was successfully removed from ${team?.title} team`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );

        getTeam(id)
          .then((res) => {
            setError("");
            setTeam(res);
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
        setRemovePlayerError(errorMessageFromAxiosError(e));
      });
  };

  const onAddManagerSubmit = () => {
    if (!addManagerInput) {
      return setAddManagerError("Select cannot be empty");
    }
    addTeamManager(id, addManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${addManagerInput.userName} was successfully added to ${team?.title} team managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        getTeam(id)
          .then((res) => {
            setTeam(res);
          })
          .catch((e) => {
            console.log(e);
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
    removeTeamManager(id, removeManagerInput.id)
      .then(() => {
        closeModal();
        const successMessage = `Player ${removeManagerInput.userName} was successfully removed from ${team?.title} team managers`;
        dispatch(
          alertActions.changeAlert({ type: "success", message: successMessage })
        );
        getTeam(id)
          .then((res) => {
            setTeam(res);
          })
          .catch((e) => {
            console.log(e);
          });
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
      {team && (
        <Card>
          <CardHeader
            title={team.title}
            subheader={<Chip color="primary" variant="outlined" label="Team" />}
          />
          {team.pictureUrl && (
            <CardMedia component="img" height="300" image={team.pictureUrl} />
          )}
          <CardContent>
            <Typography variant="body1">{team.description}</Typography>
            <Typography variant="body2" color="text.secondary">
              Created at: {new Date(team.createDate).toLocaleString()}
            </Typography>
            {isManager(user, team.ownerId, team.managers) && (
              <Typography variant="body2" color="text.secondary">
                Last edited at: {new Date(team.lastEditDate).toLocaleString()}
              </Typography>
            )}
            <br />
            <Typography variant="h6">Players:</Typography>
            {team.players && <TeamPlayerList players={team.players} />}
            {!team.players ||
              (team.players && team.players.length === 0 && (
                <Typography>No players yet. Add some!</Typography>
              ))}
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              {isManager(user, team.ownerId, team.managers) && (
                <IconButton
                  centerRipple={false}
                  onClick={() => setModalStatus(Modal.Add)}
                >
                  <Tooltip title="Add player">
                    <PersonAddAlt1Icon />
                  </Tooltip>
                </IconButton>
              )}
              {isManager(user, team.ownerId, team.managers) && (
                <IconButton
                  centerRipple={false}
                  onClick={() => setModalStatus(Modal.Remove)}
                >
                  <Tooltip title="Remove player">
                    <PersonRemoveIcon />
                  </Tooltip>
                </IconButton>
              )}
            </Box>
            {isManager(user, team.ownerId, team.managers) && (
              <IconButton
                centerRipple={false}
                onClick={() => navigate(`/editteam/${id}`)}
              >
                <Tooltip title="Edit team">
                  <EditIcon />
                </Tooltip>
              </IconButton>
            )}
            {isOwner(user, team.ownerId) && (
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
            {isOwner(user, team.ownerId) && (
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
            {isOwner(user, team.ownerId) && (
              <IconButton
                centerRipple={false}
                color="error"
                onClick={() => setModalStatus(Modal.Delete)}
              >
                <Tooltip title="Remove team">
                  <DeleteForeverIcon />
                </Tooltip>
              </IconButton>
            )}
          </CardActions>
        </Card>
      )}
      {modalStatus === Modal.Add && (
        <AddPlayerModal
          errorMessage={addPlayerError}
          searchInput={addPlayerInput}
          onSearchInputChange={setAddPlayerInput}
          onSubmit={onAddPlayerSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Remove && (
        <RemovePlayerModal
          errorMessage={removePlayerError}
          players={team?.players ?? []}
          removePlayerInput={removePlayerInput}
          onRemovePlayerInputChange={setRemovePlayerInput}
          onSubmit={onRemovePlayerSubmit}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.Delete && (
        <DeleteTeamModal
          teamId={id}
          teamTitle={team?.title ?? ""}
          onClose={closeModal}
        />
      )}
      {modalStatus === Modal.AddManager && (
        <AddManagerModal
          errorMessage={addManagerError}
          users={users?.filter((x) => x.id !== team?.ownerId) ?? []}
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
          users={team?.managers ?? []}
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

export default TeamBigCard;
