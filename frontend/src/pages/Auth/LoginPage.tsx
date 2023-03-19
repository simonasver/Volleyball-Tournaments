import { Grid } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import LoginForm from "../../components/auth/LoginForm";
import Layout from "../../components/layout/Layout";
import { useAppSelector } from "../../utils/hooks";

const LoginPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if(user) {
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
        <LoginForm />
      </Grid>
    </Layout>
  );
};

export default LoginPage;
