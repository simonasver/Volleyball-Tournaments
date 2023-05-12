import {
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Chip,
  Typography,
} from "@mui/material";
import { GameStatus, GameTeam } from "../../../utils/types";

interface GameSmallCardProps {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  createDate: string;
  status: GameStatus;
  winner: GameTeam;
  firstTeam: GameTeam;
  secondTeam: GameTeam;
  onButtonPress: () => void;
}

const GameSmallCard = (props: GameSmallCardProps) => {
  const { title, pictureUrl, description, createDate, status, winner, firstTeam, secondTeam, onButtonPress } = props;

  let statusString = "";
  switch (status) {
    case GameStatus.New:
      statusString = "New";
      break;
    case GameStatus.SingleTeam:
      statusString = "Single team";
      break;
    case GameStatus.Ready:
      statusString = "Ready to start";
      break;
    case GameStatus.Started:
      statusString = "In progress";
      break;
    case GameStatus.Finished:
      statusString = "Finished";
      break;
  }

  return (
    <Card sx={{ width: { xs: "100%", md: "70%" } }}>
      <CardHeader title={title} subheader={<><Chip label={statusString} size="small"/>{winner && <Chip color="primary" label={"Winner: " + winner.title} size="small" />}</>} />
      {pictureUrl && (
        <CardMedia
          component="img"
          height="200"
          image={pictureUrl}
        />
      )}
      <CardContent>
        <Typography variant="body1">{description}</Typography>
        {firstTeam && <Typography variant="body1">First team: {firstTeam.title}</Typography>}
        {secondTeam && <Typography variant="body1">Second team: {secondTeam.title}</Typography>}
        <Typography variant="body2" color="text.secondary">
          Created at: {createDate}
        </Typography>
      </CardContent>
      <CardActions>
        <Button onClick={onButtonPress} size="small">
          Info
        </Button>
      </CardActions>
    </Card>
  );
};

export default GameSmallCard;
