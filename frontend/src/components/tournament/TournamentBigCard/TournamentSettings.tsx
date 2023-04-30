import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
} from "@mui/material";
import React from "react";

interface TournamentSettingsProps {
  matchForThirdPlace: boolean;
  teamLimit: number;
  basic: boolean;
  pointsToWin: number;
  pointsToWinLastSet: number;
  pointDifferenceToWin: number;
  maxSets: number;
  playersPerTeam: number;
}

const TournamentSettings = (props: TournamentSettingsProps) => {
  const {
    matchForThirdPlace,
    teamLimit,
    basic,
    pointsToWin,
    pointsToWinLastSet,
    pointDifferenceToWin,
    maxSets,
    playersPerTeam,
  } = props;

  const tableSettings = React.useMemo(() => {
    return [
      {
        name: "Match for third place",
        value: matchForThirdPlace ? "Yes" : "No",
      },
      {
        name: "Team limit",
        value: teamLimit,
      },
      {
        name: "Is scoreboard complex",
        value: !basic ? "Yes" : "No",
      },
      {
        name: "Points to win",
        value: pointsToWin,
      },
      {
        name: "Points to win last set",
        value: pointsToWinLastSet,
      },
      {
        name: "Point difference to win",
        value: pointDifferenceToWin,
      },
      {
        name: "Best of x",
        value: maxSets,
      },
      {
        name: "Players per team",
        value: playersPerTeam === 0 ? "Unrestricted" : playersPerTeam,
      },
    ];
  }, [basic, pointsToWin, pointDifferenceToWin, maxSets, playersPerTeam]);

  return (
    <Table size="small">
      <TableHead>
        <TableRow>
          <TableCell>
            <b>Setting</b>
          </TableCell>
          <TableCell>
            <b>Value</b>
          </TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {tableSettings.map((setting) => (
          <TableRow key={setting.name}>
            <TableCell>{setting.name}</TableCell>
            <TableCell>{setting.value}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
};

export default TournamentSettings;
