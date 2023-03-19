import { Grid } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import Profile from "../../components/profile/Profile";
import { useAppSelector } from "../../utils/hooks";

const ProfilePage = () => {
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
        <Profile />
      </Grid>
    </Layout>
  );
};

export default ProfilePage;
