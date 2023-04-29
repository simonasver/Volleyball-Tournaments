import React from "react";
import UsersList from "../../components/admin/UsersList";
import { useAppSelector } from "../../utils/hooks";
import { Grid } from "@mui/material";
import { useNavigate } from "react-router-dom";
import Layout from "../../components/layout/Layout";
import { isAdmin } from "../../utils/helpers";

const UsersListPage = () => {
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
        <UsersList />
      </Grid>
    </Layout>
  );
};

export default UsersListPage;
