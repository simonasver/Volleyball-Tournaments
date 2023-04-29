import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import { useAppSelector } from "../../utils/hooks";
import React from "react";
import { isAdmin } from "../../utils/helpers";
import AdminPanel from "../../components/admin/AdminPanel";
import { Grid } from "@mui/material";

const AdminPanelPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if (!user || !isAdmin(user)) {
      return navigate("/", { replace: true });
    }
  }, []);

  return (
    <Layout header>
      <Grid
        container
        spacing={0}
        direction="column"
        alignItems="center"
        justifyContent="center"
      >
        <AdminPanel />
      </Grid>
    </Layout>
  );
};

export default AdminPanelPage;
