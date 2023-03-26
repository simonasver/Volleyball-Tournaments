import { Routes, Route } from "react-router-dom";
import LoginPage from "./pages/Auth/LoginPage";
import LogoutPage from "./pages/Auth/LogoutPage";
import RegisterPage from "./pages/Auth/RegisterPage";
import EmptyPage from "./pages/EmptyPage";
import AllGamesPage from "./pages/Game/AllGamesPage";
import CreateGamePage from "./pages/Game/CreateGamePage";
import GamePage from "./pages/Game/GamePage";
import MyGamesPage from "./pages/Game/MyGamesPage";
import NotFoundPage from "./pages/NotFoundPage";
import EditProfilePage from "./pages/Profile/EditProfilePage";
import ProfilePage from "./pages/Profile/ProfilePage";
import StartingPage from "./pages/StartingPage";
import CreateTeamPage from "./pages/Team/CreateTeamPage";
import EditTeamPage from "./pages/Team/EditTeamPage";
import MyTeamsPage from "./pages/Team/MyTeamsPage";
import TeamPage from "./pages/Team/TeamPage";

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
      <Route path="/team/:teamId" element={<TeamPage />} />
      <Route path="/createteam" element={<CreateTeamPage />} />
      <Route path="/editteam/:teamId" element={<EditTeamPage />} />

      <Route path="/games" element={<AllGamesPage />} />
      <Route path="/mygames" element={<MyGamesPage />} />
      <Route path="/game/:gameId" element={<GamePage />} />
      <Route path="/creategame" element={<CreateGamePage />} />

      <Route path="/empty" element={<EmptyPage />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

export default App;
