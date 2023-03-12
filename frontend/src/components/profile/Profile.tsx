import React from "react";
import {
  Alert,
  Avatar,
  Button,
  Grid,
  TextField,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { getUser } from "../../services/user.service";
import { useAppDispatch, useAppSelector } from "../../hooks";
import BackButton from "../layout/BackButton";
import Loader from "../layout/Loader";
import { authActions } from "../../store/auth-slice";
import axios from "axios";

const Profile = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(false);

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      getUser(user.id, abortController.signal)
        .then((res) => {
          console.log(res);
          dispatch(
            authActions.changeUser({
              ...user,
              fullName: res.fullName,
              profilePictureUrl: res.profilePictureUrl,
              registerDate: new Date(res.registerDate).toDateString(),
              lastLoginDate: new Date(res.lastLoginDate).toDateString(),
            })
          );
        })
        .catch((e) => {
          if(!axios.isCancel(e)){
            console.log(e);
            if (e.response) {
              setError(e.response.data.message || "Error");
            } else if (e.request) {
              setError("Connection error");
            } else {
              setError("Error");
            }
          }
        })
        .finally(() => setIsLoading(false));
    }
    return () => {
      abortController.abort();
    };
  }, []);

  const onClickHandler = () => {
    navigate("/editprofile");
  };

  return (
    <Grid item xs={3}>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      {isLoading && <Loader />}
      {!isLoading && user && (
        <>
          <Typography variant="h5">Profile</Typography>
          <br />
          <Typography variant="subtitle2">Your profile information</Typography>
          <br />
          <Grid
            container
            spacing={0}
            direction="column"
            alignItems="center"
            justifyContent="center"
          >
            <Grid item xs={1}>
              <Avatar
                alt={user.fullName}
                src={user.profilePictureUrl}
                sx={{ width: 100, height: 100 }}
              />
            </Grid>
          </Grid>
          <br />
          <TextField
            value={user.email}
            type="email"
            label="Email"
            variant="outlined"
            fullWidth
            disabled
          />
          <br />
          <br />
          <TextField
            value={user.userName}
            type="text"
            label="Username"
            variant="outlined"
            fullWidth
            disabled
          />
          <br />
          <br />
          <TextField
            value={user.fullName || ""}
            type="text"
            label="Full name"
            variant="outlined"
            fullWidth
            disabled
          />
          <br />
          <br />
          <TextField
            value={user.lastLoginDate || ""}
            type="text"
            label="Last login date"
            variant="outlined"
            fullWidth
            disabled
          />
          <br />
          <br />
          <TextField
            value={user.registerDate || ""}
            type="text"
            label="Register date"
            variant="outlined"
            fullWidth
            disabled
          />
          <br />
          <br />
          <Grid
            container
            spacing={1}
            direction="row"
            alignItems="center"
            justifyContent="space-between"
          >
            <Grid item xs={5}>
              <BackButton />
            </Grid>
            <Grid item xs={5}>
              <Button onClick={onClickHandler} variant="contained">
                Edit profile
              </Button>
            </Grid>
          </Grid>
        </>
      )}
    </Grid>
  );
};

export default Profile;
