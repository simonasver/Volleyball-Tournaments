import { Container } from "@mui/material";

interface MainContentProps {
  children: React.ReactNode;
}

const MainContent = (props: MainContentProps) => {
  return (
    <main>
      <Container sx={{ paddingTop: "24px", paddingBottom: "24px" }}>{props.children}</Container>
    </main>
  );
};

export default MainContent;
