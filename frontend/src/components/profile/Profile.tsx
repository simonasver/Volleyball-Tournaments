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
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import BackButton from "../layout/BackButton";
import Loader from "../layout/Loader";
import { authActions } from "../../store/auth-slice";
import { errorMessageFromAxiosError } from "../../utils/helpers";

const Profile = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const user = useAppSelector((state) => state.auth.user);

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      getUser(user.id, abortController.signal)
        .then((res) => {
          dispatch(
            authActions.changeUser({
              ...user,
              fullName: res.fullName,
              profilePictureUrl: res.profilePictureUrl,
              registerDate: new Date(res.registerDate).toDateString(),
              lastLoginDate: new Date(res.lastLoginDate).toDateString(),
              roles: res.userRoles,
            })
          );
          setIsLoading(false);
        })
        .catch((e) => {
          console.log(e);
          const errorMessage = errorMessageFromAxiosError(e);
          setError(errorMessage);
          if(errorMessage){
            setIsLoading(false);
          }
        })
    }
    return () => {
      abortController.abort();
    };
  }, []);

  const onClickHandler = () => {
    navigate("/editprofile");
  };

  return (
    <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <Grid
        container
        spacing={1}
        direction="row"
        alignItems="center"
        justifyContent="flex-start"
      >
        <Grid item>
          <BackButton address="/" />
        </Grid>
      </Grid>
      <br />
      <Loader isOpen={isLoading}/>
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
                sx={{
                  width: { xs: 100, md: 200 },
                  height: { xs: 100, md: 200 },
                }}
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
            justifyContent="flex-start"
          >
            <Grid item xs={12}>
              <Button
                variant="outlined"
                type="submit"
                sx={{ width: { xs: "100%", md: "inherit" } }}
                onClick={onClickHandler}
              >
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
