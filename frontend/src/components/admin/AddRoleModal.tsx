import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  SelectChangeEvent,
} from "@mui/material";
import { UserRole } from "../../store/auth-slice";
import React from "react";

interface AddRoleModalProps {
  header: string;
  onSubmit: (role: UserRole, action: boolean) => void;
  onClose: () => void;
}

const AddRoleModal = (props: AddRoleModalProps) => {
  const { header, onSubmit, onClose } = props;

  const [roleInput, setRoleInput] = React.useState<UserRole>(UserRole.Admin);

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>
        {header}
        <br />
        <br />
      </DialogTitle>
      <DialogContent>
        <br />
        <FormControl fullWidth>
          <InputLabel>Role to add</InputLabel>
          <Select
            value={roleInput}
            label="Role to add"
            onChange={(e: SelectChangeEvent<UserRole>) =>
              setRoleInput(e.target.value as UserRole)
            }
          >
            {[{ id: UserRole.Admin, value: "Admin" }].map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.value}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button
          type="submit"
          variant="contained"
          onClick={() => onSubmit(roleInput, true)}
        >
          Add
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddRoleModal;
