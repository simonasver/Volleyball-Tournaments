import { Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography } from "@mui/material";
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
    onChangeScore: (setId: string, playerId: string, change: boolean) => void;
}

const SetTable = (props: SetTableProps) => {
    const { setId, isOwner, status, teamName, teamScore, winner, players, team, onChangeScore } = props;

  return (
    <>
      <Typography
        color={
          status === GameStatus.Finished
            ? !winner
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
            ? !winner
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
            <col width="30%" />
            <col width="70%" />
          </colgroup>
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
