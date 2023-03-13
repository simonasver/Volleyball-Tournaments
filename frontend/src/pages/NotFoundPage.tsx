import { Alert, Divider, Typography, Link } from "@mui/material";
import { useNavigate } from "react-router-dom";
import Layout from "../components/layout/Layout";

const NotFoundPage = () => {
  const navigate = useNavigate();

  return (
    <Layout>
      <Alert severity="error">
        <Typography>Page not found!</Typography>
        <Divider />
        <Link href="" underline="hover" onClick={() => navigate("/")}>
          Go to our starting page
        </Link>
      </Alert>
    </Layout>
  );
};

export default NotFoundPage;
