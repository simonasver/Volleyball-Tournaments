import { Button, Link, TextField } from "@mui/material";
import Alert from "@mui/material/Alert/Alert";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import React from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "../../hooks";
import { login } from "../../services/auth.service";
import { setUserData } from "../../storage/auth.storage";
import { alertActions } from "../../store/alert-slice";

const LoginForm = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [error, setError] = React.useState(null);

  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");

  const onLoginSubmit = (event: React.FormEvent) => {
    event.preventDefault();

    login(email, password)
      .then((res) => {
        setUserData(dispatch, res.accessToken, res.refreshToken, {
          userName: res.userName,
          email: res.userEmail,
          roles: res.userRoles,
        });
        setError(null);
        dispatch(
          alertActions.changeAlert({
            type: "success",
            message: "Successfully logged in!",
          })
        );
        navigate("/", { replace: true });
      })
      .catch((e) => {
        console.log(e?.response?.data);
        setError(e?.response?.data);
      });
  };

  return (
    <>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Grid item xs={3}>
        <Typography variant="h5">Login</Typography>
        <br />
        <Typography variant="subtitle2">
          Please enter your username and password!
        </Typography>
        <br />
        <form onSubmit={onLoginSubmit}>
          <TextField
            value={email}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setEmail(e.target.value)
            }
            type="text"
            label="Username"
            variant="outlined"
            fullWidth
            required
          />
          <br />
          <br />
          <TextField
            value={password}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setPassword(e.target.value)
            }
            type="password"
            label="Password"
            variant="outlined"
            fullWidth
            required
          />
          <br />
          <br />
          <Button variant="contained" type="submit" fullWidth>
            Login
          </Button>
        </form>
        <br />
        <Link variant="subtitle1" href="/register" underline="hover">
          Do not have an account? Register!
        </Link>
      </Grid>
    </>
  );
};

export default LoginForm;
