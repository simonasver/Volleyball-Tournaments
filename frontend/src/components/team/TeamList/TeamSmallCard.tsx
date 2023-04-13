import {
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  CardMedia,
  Typography,
} from "@mui/material";

interface TeamSmallCardProps {
  title: string;
  imageUrl?: string;
  description?: string;
  createDate: string;
  onButtonPress: () => void;
}

const TeamSmallCard = (props: TeamSmallCardProps) => {
  const { title, imageUrl, description, createDate, onButtonPress } = props;
  return (
    <Card sx={{ width: { xs: "100%", md: "70%" } }}>
      <CardHeader title={title} />
      {imageUrl && (
        <CardMedia
          component="img"
          height="200"
          image={imageUrl}
        />
      )}

      <CardContent>
        <Typography variant="body1">{description}</Typography>
        <Typography variant="body2" color="text.secondary">
          Created at: {createDate}
        </Typography>
      </CardContent>
      <CardActions>
        <Button onClick={onButtonPress} size="small">Info</Button>
      </CardActions>
    </Card>
  );
};

export default TeamSmallCard;
