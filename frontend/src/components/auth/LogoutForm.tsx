import React from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../utils/hooks";
import { logout } from "../../services/auth.service";
import Loader from "../layout/Loader";
import { alertActions } from "../../store/alert-slice";

const LogoutForm = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  React.useEffect(() => {
    const abortController = new AbortController();
    logout(dispatch, abortController.signal)
      .then()
      .catch((e) => {
        console.log(e);
      })
      .finally(() => {
        dispatch(
          alertActions.changeAlert({ type: "success", message: "Successfully logged out" })
        );
        return navigate("/", { replace: true });
      });
    return () => abortController.abort();
  }, []);
  return <Loader isOpen={true}/>;
};

export default LogoutForm;
