import { Box } from "@mui/material";
import { Bracket, BracketGame } from "react-tournament-bracket";
import { TournamentMatch } from "../../../utils/types";
import { Game } from "react-tournament-bracket/lib/components/model";
import { useNavigate } from "react-router-dom";

interface TournamentBracketProps {
  tournamentGames: TournamentMatch[];
}

const MapGameDataToFrontEnd = (tournamentGame: TournamentMatch) => {
  const mappedGame: Game = {
    id: tournamentGame.id,
    name: tournamentGame.game?.title ?? "",
    bracketLabel: tournamentGame.game?.title ?? "",
    scheduled: Number(new Date()),
    sides: {
      home: {
        team: {
          id: tournamentGame.game?.firstTeam.id ?? "",
          name: tournamentGame.game?.firstTeam.title ?? "",
        },
        score: {
          score: tournamentGame.game?.firstTeamScore ?? 0,
        },
      },
      visitor: {
        team: {
          id: tournamentGame.game?.secondTeam.id ?? "",
          name: tournamentGame.game?.secondTeam.title ?? "",
        },
        score: {
          score: tournamentGame.game?.secondTeamScore ?? 0,
        },
      },
    },
  };
  return mappedGame;
};

const MapTournamentDataToFrontEnd = (tournamentGames: TournamentMatch[]) => {
  if(!tournamentGames || tournamentGames.length === 0) {
    return undefined;
  }

  const finalGame = tournamentGames.reduce((prev, current) => {
    return prev.round > current.round ? prev : current;
  });

  const mappedFinalGame = MapGameToParentGames(
    MapGameDataToFrontEnd(finalGame),
    tournamentGames
  );

  return mappedFinalGame;
};

const MapGameToParentGames = (
  mappedGame: Game,
  tournamentGames: TournamentMatch[]
) => {
  const tournamentGame = tournamentGames.find((x) => x.id === mappedGame.id);
  if (!tournamentGame) {
    return mappedGame;
  }

  if (tournamentGame.firstParent) {
    mappedGame.sides.home.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.firstParent),
        tournamentGames
      ),
      sourcePool: {},
    };
  }
  if (tournamentGame.secondParent) {
    mappedGame.sides.visitor.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.secondParent),
        tournamentGames
      ),
      sourcePool: {},
    };
  }
  return mappedGame;
};

const TournamentBracket = (props: TournamentBracketProps) => {
  const { tournamentGames } = props;

  const formattedGames = MapTournamentDataToFrontEnd(tournamentGames);

  return (
    <Box sx={{ width: "100%", overflow: "auto" }}>
      <Box sx={{ width: "fit-content", margin: "0 auto" }}>
        {formattedGames && (
          <Bracket
            game={formattedGames}
            GameComponent={TournamentBracketGameComponent}
          />
        )}
      </Box>
    </Box>
  );
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
const TournamentBracketGameComponent = (props: any) => {
  const navigate = useNavigate();
  return <BracketGame {...props} onClick={() => navigate(`/game/${props.game.id}`)} />;
};

export default TournamentBracket;
