import { Alert } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../hooks";
import { logout } from "../../services/auth.service";

const LogoutForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  React.useEffect(() => {
    logout("", "", dispatch)
      .then((res) => {
        console.log(res);
      })
      .catch((e) => {
        console.log(e?.response?.data);
      })
      .finally(() => {
        navigate("/", { replace: true });
      });
  }, []);
  return <Alert severity="info">Logging out</Alert>;
};

export default LogoutForm;
