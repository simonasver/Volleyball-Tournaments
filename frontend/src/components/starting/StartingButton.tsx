import { Button, Grid } from "@mui/material";

interface StartingButtonProps {
    title: string;
    onClick?: React.MouseEventHandler<HTMLButtonElement>;
}

const StartingButton = (props: StartingButtonProps) => {
    const { title, onClick } = props;
    return (<Grid
        item
        justifyContent="space-evenly"
        display="flex"
      >
        <Button
          variant="contained"
          sx={{
            size: { xs: "small", md: "medium" },
            whiteSpace: "nowrap",
            overflow: "hidden",
            textOverflow: "ellipsis",
          }}
          onClick={onClick}
        >
          {title}
        </Button>
      </Grid>);
};

export default StartingButton;