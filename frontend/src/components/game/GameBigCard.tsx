import {
  Card,
  CardActions,
  CardContent,
  CardHeader,
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
import { GameStatus } from "../../utils/types";
import DeleteGameModal from "./DeleteGameModal";

interface GameBigCardProps {
  id: string;
  title: string;
  description: string;
  createDate: string;
  status: GameStatus;
}

enum Modal {
  None = 0,
  Delete = 1,
}

const GameBigCard = (props: GameBigCardProps) => {
  const { id, title, description, createDate, status } = props;

  const navigate = useNavigate();

  const [modalStatus, setModalStatus] = React.useState(Modal.None);
  const [selectedSet, setSelectedSet] = React.useState(0);

  const [addPlayerInput, setAddPlayerInput] = React.useState("");
  const [removePlayerInput, setRemovePlayerInput] = React.useState("");

  const [addPlayerError, setAddPlayerError] = React.useState("");
  const [removePlayerError, setRemovePlayerError] = React.useState("");
  const [deleteTeamError, setDeleteTeamError] = React.useState("");

  const closeModal = () => {
    setAddPlayerInput("");
    setAddPlayerError("");
    setRemovePlayerInput("");
    setRemovePlayerError("");
    setDeleteTeamError("");
    setModalStatus(Modal.None);
  };

  const onSetChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedSet(newValue);
  };

  const onAddPlayerSubmit = () => {
    console.log("add");
  };

  const onRemovePlayerSubmit = () => {
    console.log("remove");
  };

  const onDeleteSubmit = () => {
    console.log("delete");
  };

  let statusString = "";
  switch (status) {
    case 0:
      statusString = "New";
      break;
    case 1:
      statusString = "In progress";
      break;
    case 2:
      statusString = "Finished";
      break;
  }

  return (
    <>
      <Card>
        <CardHeader title={title} subheader={statusString} />
        <CardContent>
          <Typography variant="body1">{description}</Typography>
          <Typography variant="body2" color="text.secondary">
            Created at: {createDate}
          </Typography>
          <Tabs value={selectedSet} onChange={onSetChange} centered>
            <Tab label="Set 1" />
            <Tab label="Set 2" />
            <Tab label="Set 3" />
          </Tabs>
        </CardContent>
        <CardActions>
          <Box sx={{ flexGrow: 1 }}>
            <IconButton
              centerRipple={false}
              onClick={() => navigate(`/editgame/${id}`)}
            >
              <Tooltip title="Edit game">
                <EditIcon />
              </Tooltip>
            </IconButton>
          </Box>
          <IconButton
            centerRipple={false}
            color="error"
            onClick={() => setModalStatus(Modal.Delete)}
          >
            <Tooltip title="Remove game">
              <DeleteForeverIcon />
            </Tooltip>
          </IconButton>
        </CardActions>
      </Card>
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
