import React from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../utils/hooks";
import { logout } from "../../services/auth.service";
import Loader from "../layout/Loader";

const LogoutForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  React.useEffect(() => {
    logout(dispatch)
      .then()
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {
        navigate("/", { replace: true });
      });
  }, []);
  return <Loader isOpen={true}/>;
};

export default LogoutForm;
