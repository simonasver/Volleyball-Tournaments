import { Grid } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import EditProfile from "../../components/profile/EditProfile";
import { useAppSelector } from "../../utils/hooks";

const EditProfilePage = () => {
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
        <EditProfile />
      </Grid>
    </Layout>
  );
};

export default EditProfilePage;
