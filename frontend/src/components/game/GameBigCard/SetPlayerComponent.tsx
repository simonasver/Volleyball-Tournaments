import React from "react";
import { Grid, IconButton, TableCell, TableRow } from "@mui/material";
import { GameStatus } from "../../../utils/types";

import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward";
import KeyboardDoubleArrowUpIcon from "@mui/icons-material/KeyboardDoubleArrowUp";
import KeyboardDoubleArrowDownIcon from "@mui/icons-material/KeyboardDoubleArrowDown";

interface SetPlayerComponentProps {
  playerId: string;
  setId: string;
  name: string;
  score: number;
  kills: number;
  errors: number;
  attempts: number;
  successfulBlocks: number;
  blocks: number;
  touches: number;
  blockingErrors: number;
  aces: number;
  servingErrors: number;
  totalServes: number;
  successfulDigs: number;
  ballTouches: number;
  ballMisses: number;
  isOwner: boolean;
  status: GameStatus;
  basic: boolean;
  onChangeScore: (
    setId: string,
    playerId: string,
    change: boolean,
    fast?: boolean
  ) => void;
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
      <TableCell
        sx={{
          left: 0,
          position: "sticky",
          backgroundColor: "#ebeced",
          boxShadow: 1,
          minWidth: { md: "300px", xs: "100px" },
        }}
      >
        <Grid
          display={"flex"}
          direction={"row"}
          justifyContent={"space-between"}
          alignItems={"center"}
        >
          <Grid item display={"inline"}>{name}</Grid>
          <Grid item display={"inline"}>
            {isOwner &&
              status >= GameStatus.Started &&
              status < GameStatus.Finished && (
                <IconButton
                  color="success"
                  component="label"
                  size="small"
                  sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                  onClick={() => onChangeScore(setId, playerId, true, true)}
                >
                  <KeyboardDoubleArrowUpIcon />
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
                  onClick={() => onChangeScore(setId, playerId, false, true)}
                >
                  <KeyboardDoubleArrowDownIcon />
                </IconButton>
              )}
            {isOwner &&
              status >= GameStatus.Started &&
              status < GameStatus.Finished &&
              !basic && (
                <IconButton
                  color="success"
                  component="label"
                  size="small"
                  sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                  onClick={() => onChangeScore(setId, playerId, true, false)}
                >
                  <ArrowUpwardIcon />
                </IconButton>
              )}
            {isOwner &&
              status >= GameStatus.Started &&
              status < GameStatus.Finished &&
              !basic && (
                <IconButton
                  color="error"
                  component="label"
                  size="small"
                  sx={{ opacity: 0.5, "&:hover": { opacity: 1 } }}
                  onClick={() => onChangeScore(setId, playerId, false, false)}
                >
                  <ArrowDownwardIcon />
                </IconButton>
              )}
          </Grid>
        </Grid>
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
