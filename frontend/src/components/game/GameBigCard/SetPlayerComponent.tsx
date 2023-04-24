import { IconButton, TableCell, TableRow } from "@mui/material";
import { GameStatus } from "../../../utils/types";

import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward";
import React from "react";

interface SetPlayerComponentProps {
  playerId: string;
  setId: string;
  name: string;
  score: number;
  kills: number,
  errors: number,
  attempts: number,
  successfulBlocks: number,
  blocks: number,
  touches: number,
  blockingErrors: number,
  aces: number,
  servingErrors: number,
  totalServes: number,
  successfulDigs: number,
  ballTouches: number,
  ballMisses: number,
  isOwner: boolean;
  status: GameStatus;
  basic: boolean;
  onChangeScore: (setId: string, playerId: string, change: boolean) => void;
}

const SetPlayerComponent = React.memo(function SetPlayer(
  props: SetPlayerComponentProps
) {
  const {
    playerId,
    setId,
    name,
    score,
    kills,
    errors,
    attempts,
    successfulBlocks,
    blocks,
    touches,
    blockingErrors,
    aces,
    servingErrors,
    totalServes,
    successfulDigs,
    ballTouches,
    ballMisses,
    isOwner,
    status,
    basic,
    onChangeScore,
  } = props;

  return (
    <TableRow key={playerId}>
      <TableCell sx={{ left: 0, position: "sticky", backgroundColor: "#ebeced", boxShadow: 1, minWidth: { md: "200px", xs: "100px" } }}>
        {name}
        {isOwner &&
          status >= GameStatus.Started &&
          status < GameStatus.Finished && (
            <IconButton
              color="success"
              component="label"
              size="small"
              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
              onClick={() => onChangeScore(setId, playerId, true)}
            >
              <ArrowUpwardIcon />
            </IconButton>
          )}
        {isOwner &&
          status >= GameStatus.Started &&
          status < GameStatus.Finished && (
            <IconButton
              color="error"
              component="label"
              size="small"
              sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
              onClick={() => onChangeScore(setId, playerId, false)}
            >
              <ArrowDownwardIcon />
            </IconButton>
          )}
      </TableCell>
      <TableCell>{score}</TableCell>
      {!basic && (
        <>
          <TableCell>{kills}</TableCell>
          <TableCell>{errors}</TableCell>
          <TableCell>{attempts}</TableCell>
          <TableCell>{successfulBlocks}</TableCell>
          <TableCell>{blocks}</TableCell>
          <TableCell>{touches}</TableCell>
          <TableCell>{blockingErrors}</TableCell>
          <TableCell>{aces}</TableCell>
          <TableCell>{servingErrors}</TableCell>
          <TableCell>{totalServes}</TableCell>
          <TableCell>{successfulDigs}</TableCell>
          <TableCell>{ballTouches}</TableCell>
          <TableCell>{ballMisses}</TableCell>
        </>
      )}
    </TableRow>
  );
});

export default SetPlayerComponent;
