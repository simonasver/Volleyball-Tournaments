import { Button, Card, CardActions, CardContent, CardHeader, CardMedia, Chip, Typography } from "@mui/material";
import { TournamentStatus } from "../../../utils/types";

interface TournamentSmallCardProps {
    id: string;
    title: string;
    pictureUrl: string;
    description: string;
    createDate: string;
    status: TournamentStatus;
    teamCount: number;
    maxTeams: number;
    onButtonPress: () => void;
  }

const TournamentSmallCard = (props: TournamentSmallCardProps) => {

    const { title, pictureUrl, description, createDate, status, teamCount, maxTeams, onButtonPress } = props;

  return (
    <Card sx={{ width: { xs: "100%", md: "50%" } }}>
      <CardHeader
        title={title}
        subheader={<Chip label={TournamentStatus[status]} size="small" />}
      />
      {pictureUrl && (
        <CardMedia
          component="img"
          height="200"
          image={pictureUrl}
        />
      )}
      <CardContent>
        <Typography variant="body1">{description}</Typography>
        <Typography variant="body1">Teams: {teamCount}/{maxTeams}</Typography>
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

export default TournamentSmallCard;
