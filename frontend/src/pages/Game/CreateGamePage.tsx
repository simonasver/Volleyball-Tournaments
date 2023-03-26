import React from "react";
import { Grid } from "@mui/material";
import { useNavigate } from "react-router-dom";
import CreateGameForm from "../../components/game/CreateGameForm";
import Layout from "../../components/layout/Layout";
import { useAppSelector } from "../../utils/hooks";

const CreateGamePage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user) {
      navigate("/", { replace: true });
    }
  }, []);

  return (
    <Layout>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <CreateGameForm />
      </Grid>
    </Layout>
  );
};

export default CreateGamePage;
