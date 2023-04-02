import {
  Alert,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
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
  deleteTeam,
  getTeam,
  removePlayerFromTeam,
} from "../../services/team.service";
import { Team } from "../../utils/types";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import Loader from "../layout/Loader";

interface TeamBigCardProps {
  id: string;
}

enum Modal {
  None = 0,
  Add = 1,
  Remove = 2,
  Delete = 3,
}

const TeamBigCard = (props: TeamBigCardProps) => {
  const { id } = props;

  const navigate = useNavigate();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);
  const [team, setTeam] = React.useState<Team>();

  const [modalStatus, setModalStatus] = React.useState(Modal.None);

  const [addPlayerInput, setAddPlayerInput] = React.useState("");
  const [removePlayerInput, setRemovePlayerInput] = React.useState("");

  const [addPlayerError, setAddPlayerError] = React.useState("");
  const [removePlayerError, setRemovePlayerError] = React.useState("");
  const [deleteTeamError, setDeleteTeamError] = React.useState("");

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

  const closeModal = () => {
    setAddPlayerInput("");
    setAddPlayerError("");
    setRemovePlayerInput("");
    setRemovePlayerError("");
    setDeleteTeamError("");
    setModalStatus(Modal.None);
  };

  const onAddPlayerSubmit = () => {
    if (!addPlayerInput) {
      return setAddPlayerError("Name cannot be empty");
    }
    addPlayerToTeam(id, addPlayerInput)
      .then(() => {
        closeModal();

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

  const onDeleteSubmit = () => {
    deleteTeam(id)
      .then(() => {
        navigate("/myteams", { replace: true });
      })
      .catch((e) => {
        console.log(e);
        setDeleteTeamError(errorMessageFromAxiosError(e));
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
          <CardHeader title={team.title} />
          {team.pictureUrl && (
            <CardMedia component="img" height="300" image={team.pictureUrl} />
          )}
          <CardContent>
            <Typography variant="body1">{team.description}</Typography>
            <Typography variant="body2" color="text.secondary">
              Created at: {team.createDate}
            </Typography>
            <br />
            <Typography variant="h6">Players:</Typography>
            {team.players &&
              team.players.map((item) => (
                <Typography key={item.id} variant="body1">
                  • {item.name}
                </Typography>
              ))}
            {!team.players ||
              (team.players && team.players.length === 0 && (
                <Typography>No players yet. Add some!</Typography>
              ))}
          </CardContent>
          <CardActions>
            <Box sx={{ flexGrow: 1 }}>
              <IconButton
                centerRipple={false}
                onClick={() => navigate(`/editteam/${id}`)}
              >
                <Tooltip title="Edit team">
                  <EditIcon />
                </Tooltip>
              </IconButton>
              <IconButton
                centerRipple={false}
                onClick={() => setModalStatus(Modal.Add)}
              >
                <Tooltip title="Add player">
                  <PersonAddAlt1Icon />
                </Tooltip>
              </IconButton>
              <IconButton
                centerRipple={false}
                onClick={() => setModalStatus(Modal.Remove)}
              >
                <Tooltip title="Remove player">
                  <PersonRemoveIcon />
                </Tooltip>
              </IconButton>
            </Box>
            <IconButton
              centerRipple={false}
              color="error"
              onClick={() => setModalStatus(Modal.Delete)}
            >
              <Tooltip title="Remove team">
                <DeleteForeverIcon />
              </Tooltip>
            </IconButton>
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
          errorMessage={deleteTeamError}
          onSubmit={onDeleteSubmit}
          onClose={closeModal}
        />
      )}
    </>
  );
};

export default TeamBigCard;
