import { List, ListItem, ListItemText } from "@mui/material";
import { TeamPlayer } from "../../../utils/types";

interface TeamPlayerListProps {
  players: TeamPlayer[];
}

const TeamPlayerList = (props: TeamPlayerListProps) => {
  const { players } = props;
  return (
    <List>
      {players.map((player, index) => (
        <ListItem key={player.id}>
          <ListItemText primary={`${index + 1}. ${player.name}`} />
        </ListItem>
      ))}
    </List>
  );
};

export default TeamPlayerList;
