import { Grid } from "@mui/material";

/**
 * Renders row in medium size and column in small size
 * @param props <Grid item></Grid>
 * @returns Starting page button row
 */
const StartingButtonRow = (props: { children: React.ReactNode }) => {
  const { children } = props;
  return (
    <>
      <Grid
        container
        justifyContent="center"
        alignItems="center"
        spacing={1}
        direction={{ xs: "column", md: "row" }}
      >
        {children}
      </Grid>
      <br />
    </>
  );
};

export default StartingButtonRow;
