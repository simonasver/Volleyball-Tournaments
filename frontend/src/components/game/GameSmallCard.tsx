import {
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Typography,
} from "@mui/material";
import { GameStatus } from "../../utils/types";

interface GameSmallCardProps {
  id: string;
  title: string;
  description: string;
  createDate: string;
  status: GameStatus;
  onButtonPress: () => void;
}

const GameSmallCard = (props: GameSmallCardProps) => {
  const { title, description, createDate, status, onButtonPress } = props;

  let statusString = "";
  switch (status) {
    case 0:
      statusString = "New";
      break;
    case 1:
      statusString = "In progress";
      break;
    case 2:
      statusString = "Finished";
      break;
  }

  return (
    <Card sx={{ width: { xs: "100%", md: "50%" } }}>
      <CardHeader title={title} subheader={statusString} />
      <CardContent>
        <Typography variant="body1">{description}</Typography>
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
