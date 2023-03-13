import { Routes, Route } from "react-router-dom";
import LoginPage from "./pages/Auth/LoginPage";
import LogoutPage from "./pages/Auth/LogoutPage";
import RegisterPage from "./pages/Auth/RegisterPage";
import NotFoundPage from "./pages/NotFoundPage";
import EditProfilePage from "./pages/Profile/EditProfilePage";
import ProfilePage from "./pages/Profile/ProfilePage";
import StartingPage from "./pages/StartingPage";
import CreateTeamPage from "./pages/Team/CreateTeamPage";
import MyTeamsPage from "./pages/Team/MyTeamsPage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<StartingPage />} />

      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/logout" element={<LogoutPage />} />

      <Route path="/profile" element={<ProfilePage />} />
      <Route path="/editprofile" element={<EditProfilePage />} />

      <Route path="/myteams" element={<MyTeamsPage />} />
      <Route path="/createteam" element={<CreateTeamPage />} />

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
