import React from "react";
import MainContent from "./MainContent";
import Navbar from "./Navbar";
import Footer from "./Footer";

interface LayoutProps {
  header: boolean;
  footer: boolean;
  children?: React.ReactNode;
}

const defaultProps: LayoutProps = {
  header: true,
  footer: true,
};

const Layout = (props: LayoutProps) => {
  const { header, footer, children } = props;
  const [navbarHeight, setNavbarHeight] = React.useState(0);
  const [footerHeight, setFooterHeight] = React.useState(0);

  return (
    <>
      {header && <Navbar onNavbarHeightChange={setNavbarHeight} />}
      <MainContent navbarHeight={navbarHeight} footerHeight={footerHeight}>{children}</MainContent>
      {footer && <Footer onFooterHeightChange={setFooterHeight} />}
    </>
  );
};

Layout.defaultProps = defaultProps;

export default Layout;
