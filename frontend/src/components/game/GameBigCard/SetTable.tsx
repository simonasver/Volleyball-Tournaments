import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import SetPlayerComponent from "./SetPlayerComponent";
import { GameStatus, SetPlayer } from "../../../utils/types";

interface SetTableProps {
  setId: string;
  isOwner: boolean;
  status: GameStatus;
  teamName: string;
  teamScore: number;
  winner: boolean | undefined;
  players: SetPlayer[];
  team: boolean;
  basic: boolean;
  onChangeScore: (setId: string, playerId: string, change: boolean) => void;
}

const SetTable = (props: SetTableProps) => {
  const {
    setId,
    isOwner,
    status,
    teamName,
    teamScore,
    winner,
    players,
    team,
    basic,
    onChangeScore,
  } = props;

  return (
    <>
      <Typography
        color={
          status === GameStatus.Finished
            ? team === winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {teamName}
      </Typography>
      <Typography
        variant="h6"
        color={
          status === GameStatus.Finished
            ? team === winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {teamScore}
      </Typography>
      <TableContainer component={Paper} sx={{ marginY: "20px" }}>
        <Table size="small">
          <colgroup>
            {basic && (
              <>
                <col width="50%" />
                <col width="50%" />
              </>
            )}
            {!basic && (
              <>
                <col width="30%" />
              </>
            )}
          </colgroup>
          <TableHead>
            <TableRow>
              <TableCell>Player</TableCell>
              <TableCell>Score</TableCell>
              {!basic && (<>
                <TableCell>Kills</TableCell>
                <TableCell>Errors</TableCell>
                <TableCell>Attempts</TableCell>
                <TableCell>Successful blocks</TableCell>
                <TableCell>Blocks</TableCell>
                <TableCell>Touches</TableCell>
                <TableCell>Blocking errors</TableCell>
                <TableCell>Total serves</TableCell>
                <TableCell>Successful digs</TableCell>
                <TableCell>Ball touches</TableCell>
                <TableCell>Ball misses</TableCell>
              </>)}
            </TableRow>
          </TableHead>
          <TableBody>
            {players &&
              players.length !== 0 &&
              players.map((player) => {
                if (player.team == team) {
                  return (
                    <SetPlayerComponent
                      key={player.id}
                      playerId={player.id}
                      setId={setId}
                      name={player.name}
                      score={player.score}
                      isOwner={isOwner}
                      status={status}
                      basic={basic}
                      onChangeScore={onChangeScore}
                    />
                  );
                }
              })}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );
};

export default SetTable;
