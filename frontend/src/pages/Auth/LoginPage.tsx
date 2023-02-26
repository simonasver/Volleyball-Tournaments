import { Grid } from "@mui/material";
import LoginForm from "../../components/auth/LoginForm";
import Layout from "../../components/layout/Layout";

const LoginPage = () => {
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
