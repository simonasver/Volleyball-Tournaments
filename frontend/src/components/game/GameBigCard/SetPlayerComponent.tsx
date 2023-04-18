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
  isOwner: boolean;
  status: GameStatus;
  basic: boolean;
  onChangeScore: (setId: string, playerId: string, change: boolean) => void;
}

const SetPlayerComponent = React.memo(function SetPlayer(
  props: SetPlayerComponentProps
) {
  const { playerId, setId, name, score, isOwner, status, basic, onChangeScore } =
    props;

  return (
    <TableRow key={playerId}>
      <TableCell>{name}</TableCell>
      <TableCell>
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
              <ArrowUpwardIcon fontSize="small" />
            </IconButton>
          )}
        {score}
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
              <ArrowDownwardIcon fontSize="small" />
            </IconButton>
          )}
      </TableCell>
      {!basic && (
        <>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
          <TableCell>0</TableCell>
        </>
      )}
    </TableRow>
  );
});

export default SetPlayerComponent;
