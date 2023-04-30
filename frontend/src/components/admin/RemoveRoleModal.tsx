import {
  Dialog,
  DialogTitle,
  DialogContent,
  FormControl,
  InputLabel,
  Select,
  SelectChangeEvent,
  MenuItem,
  DialogActions,
  Button,
} from "@mui/material";
import React from "react";
import { UserRole } from "../../store/auth-slice";

interface RemoveRoleModalProps {
  header: string;
  onSubmit: (role: UserRole, action: boolean) => void;
  onClose: () => void;
}

const RemoveRoleModal = (props: RemoveRoleModalProps) => {
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
          <InputLabel>Role to remove</InputLabel>
          <Select
            value={roleInput}
            label="Role to remove"
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
          onClick={() => onSubmit(roleInput, false)}
        >
          Remove
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemoveRoleModal;
