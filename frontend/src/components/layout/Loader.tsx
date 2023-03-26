import React from "react";
import { Backdrop, CircularProgress } from "@mui/material";

interface LoaderProps {
  isOpen: boolean;
}

const Loader = (props: LoaderProps) => {
  const { isOpen } = props;

  return (
    <Backdrop
      open={isOpen}
      onClick={() => undefined}
      sx={{ zIndex: 1000 }}
    >
      <CircularProgress color="primary" />
    </Backdrop>
  );
};

export default Loader;
