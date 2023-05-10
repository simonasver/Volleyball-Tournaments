import { TextField, InputAdornment, IconButton } from "@mui/material";

import ClearIcon from "@mui/icons-material/Clear";

interface SearchFilterInputProps {
    label: string;
    searchInput: string;
    onSearchInputChange: (newValue: string) => void;
}

const SearchFilterInput = (props: SearchFilterInputProps) => {
    const { label, searchInput, onSearchInputChange } = props;
    return <TextField
    size="small"
    label={label}
    variant="outlined"
    onChange={(event: React.ChangeEvent<HTMLInputElement>) =>
      onSearchInputChange(event.target.value)
    }
    value={searchInput}
    InputProps={{
      endAdornment: (
        <InputAdornment position="end">
          <IconButton
            onClick={() => onSearchInputChange("")}
            size="small"
          >
            <ClearIcon fontSize="small" />
          </IconButton>
        </InputAdornment>
      ),
    }}
  />;
};

export default SearchFilterInput;