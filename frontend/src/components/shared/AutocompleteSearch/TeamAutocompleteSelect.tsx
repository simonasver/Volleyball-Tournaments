import {
  Autocomplete,
  Box,
  Avatar,
  Typography,
  TextField,
} from "@mui/material";
import { Team } from "../../../utils/types";

interface TeamAutocompleteSelectProps {
  label: string;
  data: Team[];
  selectedTeam: Team | undefined;
  onSelectedTeamChange: (newValue: Team | undefined) => void;
}

const TeamAutocompleteSelect = (props: TeamAutocompleteSelectProps) => {
  const { label, data, selectedTeam, onSelectedTeamChange } = props;
  return (
    <Autocomplete
      value={selectedTeam}
      onChange={(e: React.SyntheticEvent, newValue: Team | null) =>
        onSelectedTeamChange(newValue ?? undefined)
      }
      options={data}
      getOptionLabel={(option) => option.title}
      renderOption={(props, option) => (
        <Box component="li" {...props}>
          <Avatar alt={option.title} src={option.pictureUrl} sizes="small" />
          <Typography sx={{ marginLeft: "10px" }} fontWeight="bold">
            {option.title}{" "}
          </Typography>
          <Typography sx={{ marginLeft: "5px" }}>
            {option.players.length ?? 0} player(s)
          </Typography>
        </Box>
      )}
      renderInput={(params) => (
        <TextField
          {...params}
          label={label}
        />
      )}
    />
  );
};

export default TeamAutocompleteSelect;
