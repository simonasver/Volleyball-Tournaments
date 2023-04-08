import { Grid } from "@mui/material";
import EditGameForm from "../../components/game/EditGameForm";
import Layout from "../../components/layout/Layout";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import React from "react";

const EditGamePage = () => {
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
        <EditGameForm />
      </Grid>
    </Layout>
  );
};

export default EditGamePage;
