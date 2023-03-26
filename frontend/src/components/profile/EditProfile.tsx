import { Alert, Button, Grid, TextField, Typography } from "@mui/material";
import React from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import { editUser, getUser } from "../../services/user.service";
import { authActions } from "../../store/auth-slice";
import BackButton from "../layout/BackButton";
import { errorMessageFromAxiosError } from "../../utils/helpers";
import Loader from "../layout/Loader";

const EditProfile = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const [error, setError] = React.useState("");
  const [isLoading, setIsLoading] = React.useState(true);

  const user = useAppSelector((state) => state.auth.user);

  const [profilePicture, setProfilePicture] = React.useState("");
  const [fullName, setFullName] = React.useState("");

  React.useEffect(() => {
    const abortController = new AbortController();
    if (!user) {
      return navigate("/", { replace: true });
    } else {
      getUser(user.id, abortController.signal)
        .then((res) => {
          setFullName(res.fullName);
          setProfilePicture(res.profilePictureUrl);
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

  const onEditSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    if (!user) {
      return navigate("/", { replace: true });
    }
    editUser(user.id, profilePicture, fullName)
      .then(() => {
        dispatch(
          authActions.changeUser({
            ...user,
            profilePictureUrl: profilePicture,
            fullName: fullName,
          })
        );
        navigate("/profile", { replace: true });
      })
      .catch((e) => {
        console.log(e);
        setError(errorMessageFromAxiosError(e));
      });
  };

  return (
    <Grid item sx={{ width: { xs: "100%", md: "50%" } }}>
      {error && (
        <>
          <Alert severity="error">{error}</Alert>
          <br />
        </>
      )}
      <>
        <Grid
          container
          spacing={1}
          direction="row"
          alignItems="center"
          justifyContent="flex-start"
        >
          <Loader isOpen={isLoading}/>
          <Grid item>
            <BackButton title="Profile" address="/profile" />
          </Grid>
        </Grid>
        <br />
        <Typography variant="h5">Edit profile</Typography>
        <br />
        <Typography variant="subtitle2">
          You can edit your profile here!
        </Typography>
        <br />
        <form onSubmit={onEditSubmit}>
          <TextField
            value={profilePicture}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setProfilePicture(e.target.value)
            }
            type="url"
            label="Profile picture (url)"
            variant="outlined"
            fullWidth
          />
          <br />
          <br />
          <TextField
            value={fullName}
            onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
              setFullName(e.target.value)
            }
            type="text"
            label="Full name"
            variant="outlined"
            fullWidth
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
                Save
              </Button>
            </Grid>
          </Grid>
        </form>
      </>
    </Grid>
  );
};

export default EditProfile;
