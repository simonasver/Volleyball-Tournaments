import React from "react";
import Layout from "../../components/layout/Layout";
import BackButton from "../../components/layout/BackButton";
import GameBigCard from "../../components/game/GameBigCard";
import { Grid } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";

const GamePage = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();

  React.useEffect(() => {
    if (!gameId) {
      navigate("/", { replace: true });
    }
  });

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <Grid item sx={{ width: { xs: "100%", md: "70%" } }}>
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="flex-start"
          >
            <Grid item>
              <BackButton title="All games" address="/games" />
            </Grid>
          </Grid>
          <br />
          {gameId && <GameBigCard id={gameId} />}
        </Grid>
      </Grid>
    </Layout>
  );
};

export default GamePage;
