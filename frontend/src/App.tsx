import { Routes, Route } from "react-router-dom";
import LoginPage from "./pages/Auth/LoginPage";
import NotFoundPage from "./pages/NotFoundPage";
import StartingPage from "./pages/StartingPage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<StartingPage />} />

      <Route path="/login" element={<LoginPage />} />

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
