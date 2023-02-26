import { Alert, Divider, Typography, Link } from "@mui/material";
import Layout from "../components/layout/Layout";

const NotFoundPage = () => {
  return (
    <Layout>
      <Alert severity="error">
        <Typography>Page not found!</Typography>
        <Divider />
        <Link href="/" underline="hover">
          Go to our starting page
        </Link>
      </Alert>
    </Layout>
  );
};

export default NotFoundPage;
