import { Grid } from "@mui/material";
import RegisterForm from "../../components/auth/RegisterForm";
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
        <RegisterForm />
      </Grid>
    </Layout>
  );
};

export default LoginPage;
