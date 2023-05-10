import {
    Autocomplete,
    Box,
    Avatar,
    Typography,
    TextField,
  } from "@mui/material";
import { User } from "../../../store/auth-slice";
  
  interface UserAutocompleteSelectProps {
    label: string;
    data: User[];
    selectedUser: User | undefined;
    onSelectedUserChange: (newValue: User | undefined) => void;
    searchInput: string;
    onSearchInputChange: (newValue: string) => void;
  }
  
  const UserAutocompleteSelect = (props: UserAutocompleteSelectProps) => {
    const { label, data, selectedUser, onSelectedUserChange, searchInput, onSearchInputChange } = props;
    return (
      <Autocomplete
        filterOptions={(x) => x}
        value={selectedUser}
        onChange={(e: React.SyntheticEvent, newValue: User | null) =>
          onSelectedUserChange(newValue ?? undefined)
        }
        options={data}
        getOptionLabel={(option) => option.userName}
        renderOption={(props, option) => (
          <Box component="li" {...props}>
            <Avatar alt={option.fullName} src={option.profilePictureUrl} sizes="small" />
            <Typography sx={{ marginLeft: "10px" }} fontWeight="bold">
              {option.fullName || option.userName}{" "}
            </Typography>
            <Typography variant="body2" color={"secondary"} sx={{ marginLeft: "5px" }}>
              @{option.userName}
            </Typography>
          </Box>
        )}
        renderInput={(params) => (
          <TextField
            {...params}
            label={label}
            value={searchInput}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
                onSearchInputChange(e.target.value)}
          />
        )}
      />
    );
  };
  
  export default UserAutocompleteSelect;
  