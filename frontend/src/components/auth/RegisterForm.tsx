import { Button, Divider, Link, TextField } from "@mui/material";
import Alert from "@mui/material/Alert/Alert";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import React from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../../services/auth.service";
import { errorMessageFromAxiosError } from "../../utils/helpers";

const RegisterForm = () => {
  const navigate = useNavigate();

  const [alertType, setAlertType] = React.useState<
    "success" | "info" | "warning" | "error"
  >("error");
  const [alert, setAlert] = React.useState<JSX.Element | undefined>(undefined);

  const [username, setUsername] = React.useState("");
  const [fullname, setFullname] = React.useState("");
  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");

  const onLoginSubmit = (event: React.FormEvent) => {
    event.preventDefault();

    register(username, fullname, email, password)
      .then((res) => {
        console.log(res);
        setAlertType("success");
        const successfulElement = (
          <>
            <Typography>Successfully created an account!</Typography>
            <Divider />
            <Link href="/login" underline="hover" onClick={() => navigate("/login", { replace: true })}>
              You can now login
            </Link>
          </>
        );
        setAlert(successfulElement);
      })
      .catch((e) => {
        const errorMessage = errorMessageFromAxiosError(e);
        setAlertType("error");
        const errorElement = <Typography>{errorMessage}</Typography>;
        setAlert(errorElement);
      });
  };

  return (
    <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
      {alert && (
        <>
          <Alert severity={alertType}>{alert}</Alert>
          <br />
        </>
      )}
      <Typography variant="h5">Register</Typography>
      <br />
      <Typography variant="subtitle2">
        Please enter your username, email and password!
      </Typography>
      <br />
      <form onSubmit={onLoginSubmit}>
        <TextField
          value={username}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setUsername(e.target.value)
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
          value={fullname}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setFullname(e.target.value)
          }
          type="text"
          label="Full name"
          variant="outlined"
          fullWidth
        />
        <br />
        <br />
        <TextField
          value={email}
          onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
            setEmail(e.target.value)
          }
          type="text"
          label="Email"
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
        <Grid
          container
          spacing={1}
          direction="row"
          alignItems="center"
          justifyContent="flex-start"
        >
          <Grid item xs={12}>
            <Button
              variant="contained"
              type="submit"
              sx={{ width: { xs: "100%", md: "inherit" } }}
            >
              Register
            </Button>
          </Grid>
        </Grid>
      </form>
      <br />
      <Link variant="subtitle1" href="/login" underline="hover" onClick={() => navigate("/login")}>
        Already have an account? Login!
      </Link>
    </Grid>
  );
};

export default RegisterForm;
