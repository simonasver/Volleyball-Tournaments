import React from "react";
import {
  Avatar,
  Button,
  IconButton,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
} from "@mui/material";
import { GameTeam } from "../../../utils/types";

import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";

interface TournamentTeamListProps {
  isOwner: boolean;
  canReorder: boolean;
  teams: GameTeam[];
  onChangeTeamOrder: (teams: GameTeam[]) => void;
}

const TournamentTeamList = (props: TournamentTeamListProps) => {
  const { isOwner, canReorder, teams, onChangeTeamOrder } = props;

  const [currentTeams, setCurrentTeams] = React.useState<GameTeam[]>(
    teams.filter((x) => !x.duplicate)
  );
  const [anyChanges, setAnyChanges] = React.useState(false);

  const onMoveUp = (idInArray: number) => {
    if (idInArray <= 0) {
      return;
    }

    setCurrentTeams((prevState) => {
      const newState = [...prevState];
      const temp = newState[idInArray - 1];
      newState[idInArray - 1] = newState[idInArray];
      newState[idInArray] = temp;
      return newState;
    });
    setAnyChanges(true);
  };

  const onMoveDown = (idInArray: number) => {
    if (idInArray >= currentTeams.length - 1) {
      return;
    }

    setCurrentTeams((prevState) => {
      const newState = [...prevState];
      const temp = newState[idInArray + 1];
      newState[idInArray + 1] = newState[idInArray];
      newState[idInArray] = temp;
      return newState;
    });
    setAnyChanges(true);
  };

  React.useEffect(() => {
    setCurrentTeams(teams.filter((x) => !x.duplicate));
    setAnyChanges(false);
  }, [teams]);

  return (
    <List>
      {currentTeams.map((team, index) => (
        <ListItem
          key={team.id}
          secondaryAction={
            <>
              {isOwner && (
                <IconButton
                  edge="end"
                  sx={{ marginRight: { xs: "0px", md: "10px" }}}
                  size="small"
                  onClick={() => onMoveUp(index)}
                  title="Move team up"
                  disabled={index <= 0 || !canReorder}
                >
                  <KeyboardArrowUpIcon />
                </IconButton>
              )}
              {isOwner && (
                <IconButton
                  edge="end"
                  size="small"
                  onClick={() => onMoveDown(index)}
                  title="Move team down"
                  disabled={index >= currentTeams.length - 1 || !canReorder}
                >
                  <KeyboardArrowDownIcon />
                </IconButton>
              )}
            </>
          }
        >
          <ListItemAvatar>
            <Avatar alt={team.title} src={team.profilePicture} />
          </ListItemAvatar>
          <ListItemText primary={`${index + 1}. ${team.title}`} />
        </ListItem>
      ))}
      {anyChanges && (
        <>
          <br />
          <Button
            variant="outlined"
            onClick={() => {
              onChangeTeamOrder(currentTeams);
              setAnyChanges(false);
            }}
          >
            Save order changes
          </Button>
        </>
      )}
    </List>
  );
};

export default TournamentTeamList;
