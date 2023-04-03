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
import { GameStatus, SetPlayer } from "../../utils/types";

import SetPlayerComponent from "./SetPlayerComponent";
import React from "react";

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

const SetTable = React.memo(function SetTable(props: SetTableProps) {
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
      <Typography
        color={
          status === GameStatus.Finished
            ? !winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {firstTeamName}
      </Typography>
      <Typography
        variant="h6"
        color={
          status === GameStatus.Finished
            ? !winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {firstTeamScore}
      </Typography>
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
                    <SetPlayerComponent
                      key={player.id}
                      playerId={player.id}
                      setId={setId}
                      name={player.name}
                      score={player.score}
                      isOwner={isOwner}
                      status={status}
                      onChangeScore={onChangeScore}
                    />
                  );
                }
              })}
          </TableBody>
        </Table>
      </TableContainer>
      <br />
      <Typography
        color={
          status === GameStatus.Finished
            ? winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {secondTeamName}
      </Typography>
      <Typography
        variant="h6"
        color={
          status === GameStatus.Finished
            ? winner
              ? "green"
              : "red"
            : "default"
        }
      >
        {secondTeamScore}
      </Typography>
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
                    <SetPlayerComponent
                      key={player.id}
                      playerId={player.id}
                      setId={setId}
                      name={player.name}
                      score={player.score}
                      isOwner={isOwner}
                      status={status}
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
});

export default SetTable;
