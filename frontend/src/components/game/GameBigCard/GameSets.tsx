import React from "react";
import { Tab, Tabs } from "@mui/material";
import GameSetComponent from "./GameSetComponent";
import { GameSet, GameStatus } from "../../../utils/types";

interface GameSetsProps {
  isOwner: boolean;
  sets: GameSet[];
  onChangeScore: (setId: string, playerId: string, change: boolean) => void;
}

const GameSets = (props: GameSetsProps) => {
  const { isOwner, sets, onChangeScore } = props;

  const [selectedSet, setSelectedSet] = React.useState(0);

  const onSetChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedSet(newValue);
  };

  return (
    <>
      {sets && sets.length !== 0 && (
        <>
          <Tabs
            value={selectedSet}
            onChange={onSetChange}
            variant="scrollable"
            scrollButtons
          >
            {sets.map((set, index) => {
              return (
                <Tab
                  key={set.id}
                  label={`Set ${index + 1}`}
                  disabled={set.status < GameStatus.Started}
                  sx={{ minWidth: "fit-content", flex: 1 }}
                />
              );
            })}
          </Tabs>
          {sets.map((set, index) => {
            return (
              <div key={set.id} hidden={selectedSet !== index}>
                <GameSetComponent
                  setId={set.id}
                  isOwner={isOwner}
                  firstTeamName={set.firstTeam.title}
                  firstTeamScore={set.firstTeamScore}
                  secondTeamName={set.secondTeam.title}
                  secondTeamScore={set.secondTeamScore}
                  players={set.players}
                  status={set.status}
                  startDate={new Date(set.startDate).toDateString()}
                  winner={set.winner ? set.winner.id === set.firstTeam.id ? false : true : undefined}
                  onChangeScore={onChangeScore}
                />
              </div>
            );
          })}
        </>
      )}
    </>
  );
};

export default GameSets;
