import MainContent from "./MainContent";
import Navbar from "./Navbar";

interface LayoutProps {
  header: boolean;
  children?: React.ReactNode;
}

const defaultProps: LayoutProps = {
  header: true,
};

const Layout = (props: LayoutProps) => {
  return (
    <>
      {props.header && <Navbar />}
      <MainContent>{props.children}</MainContent>
    </>
  );
};

Layout.defaultProps = defaultProps;

export default Layout;
