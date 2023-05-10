import {
  Autocomplete,
  Box,
  Avatar,
  Typography,
  TextField,
} from "@mui/material";
import { GameTeam } from "../../../utils/types";
import React from "react";

interface GameTeamAutocompleteSelectProps {
  label: string;
  data: GameTeam[];
  selectedTeam: GameTeam | undefined;
  onSelectedTeamChange: (newValue: GameTeam | undefined) => void;
}

const GameTeamAutocompleteSelect = (props: GameTeamAutocompleteSelectProps) => {
  const { label, data, selectedTeam, onSelectedTeamChange } = props;
  return (
    <Autocomplete
      value={selectedTeam}
      onChange={(e: React.SyntheticEvent, newValue: GameTeam | null) =>
        onSelectedTeamChange(newValue ?? undefined)
      }
      options={data}
      getOptionLabel={(option) => option.title}
      renderOption={(props, option) => (
        <Box component="li" {...props}>
          <Avatar alt={option.title} src={option.profilePicture} sizes="small" />
          <Typography sx={{ marginLeft: "10px" }} fontWeight="bold">
            {option.title}{" "}
          </Typography>
          <Typography sx={{ marginLeft: "5px" }}>
            {option.players.length ?? 0} player(s)
          </Typography>
        </Box>
      )}
      renderInput={(params) => <TextField {...params} label={label} />}
    />
  );
};

export default GameTeamAutocompleteSelect;
