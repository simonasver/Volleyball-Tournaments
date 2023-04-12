import { Box, Container, Toolbar, Typography } from "@mui/material";
import React from "react";

interface FooterProps {
    onFooterHeightChange: (newHeight: number) => void;
}

const Footer = (props: FooterProps) => {
    const { onFooterHeightChange } = props;

  const resizeElementRef = React.createRef<HTMLDivElement>();

  React.useEffect(() => {
    const element = resizeElementRef.current;
    if (!element) return;

    const observer = new ResizeObserver(() => {
      onFooterHeightChange(element.offsetHeight);
    });
    observer.observe(element);

    return () => {
      observer.disconnect();
    };
  }, []);

  return (
    <Box ref={resizeElementRef} position="static" sx={{ backgroundColor: "#1B1C20", color: "#e7e5e5" }} boxShadow={3}>
      <Container maxWidth="md">
        <Toolbar variant="dense"><Typography variant="body1">© Simonas Verenius, 2023</Typography></Toolbar>
      </Container>
    </Box>
  );
};

export default Footer;
