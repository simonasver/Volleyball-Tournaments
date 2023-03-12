import React from "react";
import { useNavigate } from "react-router-dom";
import LogoutForm from "../../components/auth/LogoutForm";
import Layout from "../../components/layout/Layout";
import { useAppSelector } from "../../hooks";

const LogoutPage = () => {
  const navigate = useNavigate();

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    if(!user) {
      navigate("/", { replace: true });
    }
  }, []);

  return (
    <Layout>
      <LogoutForm />
    </Layout>
  );
};

export default LogoutPage;
