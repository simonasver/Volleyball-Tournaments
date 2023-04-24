import React from "react";
import {
  Alert,
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
  Typography,
} from "@mui/material";
import { GameScore } from "../../../utils/types";

interface ChangeScoreModalProps {
  errorMessage: string;
  basic: boolean;
  header: string;
  positive: boolean;
  changeScoreInput: GameScore;
  changeScoreInputChange: (value: GameScore) => void;
  onSubmit: (type: GameScore, change: boolean) => void;
  onClose: () => void;
}

const ChangeScoreModal = (props: ChangeScoreModalProps) => {
  const {
    errorMessage,
    basic,
    header,
    positive,
    changeScoreInput,
    changeScoreInputChange,
    onSubmit,
    onClose,
  } = props;

  return (
    <Dialog open onClose={onClose} fullWidth>
      <DialogTitle>
        {header}
        <br />
        <br />
        <Typography variant="subtitle1" color="primary">
          Only changing score value will change team score in the set!
        </Typography>
      </DialogTitle>
      <DialogContent>
        {errorMessage && (
          <>
            <Alert severity="error">{errorMessage}</Alert>
            <br />
          </>
        )}
        <br />
        <FormControl fullWidth>
          <InputLabel>Score type</InputLabel>
          <Select
            value={changeScoreInput}
            label="Score type"
            onChange={(e: SelectChangeEvent<GameScore>) =>
              changeScoreInputChange(e.target.value as GameScore)
            }
          >
            {!basic && [
              {
                id: GameScore.Score,
                value: "Score",
              },
              {
                id: GameScore.Kills,
                value: "Kills"
              },
              {
                id: GameScore.Errors,
                value: "Errors",
              },
              {
                id: GameScore.Attempts,
                value: "Attempts",
              },
              {
                id: GameScore.SuccessfulBlocks,
                value: "Successful Blocks",
              },
              {
                id: GameScore.Blocks,
                value: "Blocks",
              },
              {
                id: GameScore.Touches,
                value: "Touches",
              },
              {
                id: GameScore.BlockingErrors,
                value: "Blocking Errors",
              },
              {
                id: GameScore.Aces,
                value: "Aces",
              },
              {
                id: GameScore.ServingErrors,
                value: "Serving Errors",
              },
              {
                id: GameScore.TotalServes,
                value: "Total Serves",
              },
              {
                id: GameScore.SuccessfulDigs,
                value: "Successful Digs",
              },
              {
                id: GameScore.BallTouches,
                value: "Ball touches",
              },
              {
                id: GameScore.BallMisses,
                value: "Ball misses",
              },
            ].map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.id === GameScore.Score ? <b>{item.value}</b> : item.value}
              </MenuItem>
            ))}
            {basic && (
              <MenuItem key={GameScore.Score} value={GameScore.Score}>
                <b>Score</b>
              </MenuItem>
            )}
          </Select>
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
        <Button type="submit" variant="contained" onClick={() => onSubmit(changeScoreInput, positive)}>
          Change
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ChangeScoreModal;
