import { AppBar, Container, Toolbar } from "@mui/material";
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
    <AppBar ref={resizeElementRef} position="static">
      <Container maxWidth="xl">
        <Toolbar disableGutters>Footer</Toolbar>
      </Container>
    </AppBar>
  );
};

export default Footer;
