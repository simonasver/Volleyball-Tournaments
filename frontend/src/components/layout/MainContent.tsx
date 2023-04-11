import { Container } from "@mui/material";

interface MainContentProps {
  navbarHeight?: number;
  footerHeight?: number;
  children: React.ReactNode;
}

const MainContent = (props: MainContentProps) => {
  const { navbarHeight, footerHeight, children } = props;
  return (
    <main>
      <Container sx={{ marginTop: `${navbarHeight}px`, paddingTop: "24px", paddingBottom: "24px", minHeight: `calc(100vh - ${navbarHeight}px - ${footerHeight}px)`, backgroundColor: "#e7e5e5" }}>{children}</Container>
    </main>
  );
};

export default MainContent;
