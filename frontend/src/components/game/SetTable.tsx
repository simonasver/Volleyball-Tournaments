import {
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { GameStatus, SetPlayer } from "../../utils/types";

import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward";

interface SetTableProps {
  setId: string;
  isOwner: boolean;
  firstTeamName: string;
  firstTeamScore: number;
  secondTeamName: string;
  secondTeamScore: number;
  players: SetPlayer[];
  status: GameStatus;
  startDate: string;
  onChangeScore: (setId: string, playerId: string, change: boolean) => void;
  winner?: boolean;
}

const SetTable = (props: SetTableProps) => {
  const {
    setId,
    isOwner,
    firstTeamName,
    firstTeamScore,
    secondTeamName,
    secondTeamScore,
    players,
    status,
    startDate,
    onChangeScore,
    winner,
  } = props;

  return (
    <>
      <br />
      <Typography variant="body2" color="text.secondary">
        Started at: {startDate}
      </Typography>
      <br />
      <Typography color={status === GameStatus.Finished ? !winner ? "green" : "red" : "default"}>{firstTeamName}</Typography>
      <Typography variant="h6" color={status === GameStatus.Finished ? !winner ? "green" : "red" : "default"}>{firstTeamScore}</Typography>
      <TableContainer component={Paper} sx={{ marginY: "20px" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Player</TableCell>
              <TableCell>Score</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {players &&
              players.length !== 0 &&
              players.map((player) => {
                if (!player.team) {
                  return (
                    <TableRow key={player.id}>
                      <TableCell>{player.name}</TableCell>
                      <TableCell>
                        {isOwner &&
                          status >= GameStatus.Started &&
                          status < GameStatus.Finished && (
                            <IconButton
                              color="error"
                              component="label"
                              size="small"
                              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                              onClick={() =>
                                onChangeScore(setId, player.id, false)
                              }
                            >
                              <ArrowDownwardIcon fontSize="small" />
                            </IconButton>
                          )}
                        {player.score}
                        {isOwner &&
                          status >= GameStatus.Started &&
                          status < GameStatus.Finished && (
                            <IconButton
                              color="success"
                              component="label"
                              size="small"
                              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                              onClick={() =>
                                onChangeScore(setId, player.id, true)
                              }
                            >
                              <ArrowUpwardIcon fontSize="small" />
                            </IconButton>
                          )}
                      </TableCell>
                    </TableRow>
                  );
                }
              })}
          </TableBody>
        </Table>
      </TableContainer>
      <br />
      <Typography color={status === GameStatus.Finished ? winner ? "green" : "red" : "default"}>{secondTeamName}</Typography>
      <Typography variant="h6"  color={status === GameStatus.Finished ? winner ? "green" : "red" : "default"}>{secondTeamScore}</Typography>
      <TableContainer component={Paper} sx={{ marginY: "20px" }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Player</TableCell>
              <TableCell>Score</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {players &&
              players.length !== 0 &&
              players.map((player) => {
                if (player.team) {
                  return (
                    <TableRow key={player.id}>
                      <TableCell>{player.name}</TableCell>
                      <TableCell>
                        {isOwner &&
                          status >= GameStatus.Started &&
                          status < GameStatus.Finished && (
                            <IconButton
                              color="error"
                              component="label"
                              size="small"
                              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                              onClick={() =>
                                onChangeScore(setId, player.id, false)
                              }
                            >
                              <ArrowDownwardIcon fontSize="small" />
                            </IconButton>
                          )}
                        {player.score}
                        {isOwner &&
                          status >= GameStatus.Started &&
                          status < GameStatus.Finished && (
                            <IconButton
                              color="success"
                              component="label"
                              size="small"
                              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                              onClick={() =>
                                onChangeScore(setId, player.id, true)
                              }
                            >
                              <ArrowUpwardIcon fontSize="small" />
                            </IconButton>
                          )}
                      </TableCell>
                    </TableRow>
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
