import { Typography } from "@mui/material";
import { GameStatus, SetPlayer } from "../../../utils/types";

import React from "react";
import SetTable from "./SetTable";

interface GameSet {
  setId: string;
  isOwner: boolean;
  firstTeamName: string;
  firstTeamScore: number;
  secondTeamName: string;
  secondTeamScore: number;
  players: SetPlayer[];
  status: GameStatus;
  startDate: string;
  basic: boolean;
  onChangeScore: (setId: string, playerId: string, change: boolean, fast?: boolean) => void;
  winner?: boolean;
}

const GameSetComponent = React.memo(function GameSetComponent(props: GameSet) {
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
    basic,
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
      <SetTable
        setId={setId}
        isOwner={isOwner}
        status={status}
        teamName={firstTeamName}
        teamScore={firstTeamScore}
        winner={winner}
        players={players}
        team={false}
        basic={basic}
        onChangeScore={onChangeScore}
      />
      <br />
      <SetTable
        setId={setId}
        isOwner={isOwner}
        status={status}
        teamName={secondTeamName}
        teamScore={secondTeamScore}
        winner={winner}
        players={players}
        team={true}
        basic={basic}
        onChangeScore={onChangeScore}
      />
    </>
  );
});

export default GameSetComponent;
