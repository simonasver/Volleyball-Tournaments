import { Grid } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import RegisterForm from "../../components/auth/RegisterForm";
import Layout from "../../components/layout/Layout";
import { useAppSelector } from "../../hooks";

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
        <RegisterForm />
      </Grid>
    </Layout>
  );
};

export default LoginPage;
