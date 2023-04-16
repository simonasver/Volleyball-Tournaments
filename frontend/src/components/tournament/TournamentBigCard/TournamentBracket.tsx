import { Box } from "@mui/material";
import { Bracket, BracketGame } from "react-tournament-bracket";
import { TournamentMatch } from "../../../utils/types";
import { Game } from "react-tournament-bracket/lib/components/model";
import { useNavigate } from "react-router-dom";

interface TournamentBracketProps {
  tournamentGames: TournamentMatch[];
}

interface ExtendedGame extends Game {
  gameId: string;
}

const MapGameDataToFrontEnd = (tournamentMatch: TournamentMatch) => {
  const mappedGame: ExtendedGame = {
    id: tournamentMatch.id ?? "",
    gameId: tournamentMatch.game?.id ?? "",
    name: tournamentMatch.game?.title ?? "",
    bracketLabel: "",
    scheduled: Math.max(Number(new Date(tournamentMatch.game?.createDate ?? "")), Number(new Date(tournamentMatch.game?.startDate ?? -8640000000000000))),
    sides: {
      home: {
        team: {
          id: tournamentMatch.game?.firstTeam?.id ?? "",
          name: tournamentMatch.game?.firstTeam?.title ?? "",
        },
        score: {
          score: tournamentMatch.game?.firstTeamScore ?? 0,
        },
      },
      visitor: {
        team: {
          id: tournamentMatch.game?.secondTeam?.id ?? "",
          name: tournamentMatch.game?.secondTeam?.title ?? "",
        },
        score: {
          score: tournamentMatch.game?.secondTeamScore ?? 0,
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

  const finalGame = tournamentGames.filter(x => !x.child)[0];

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

  if (tournamentGame.parents[0]) {
    mappedGame.sides.home.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.parents[0]),
        tournamentGames
      ),
      sourcePool: {},
    };
  }
  if (tournamentGame.parents[1]) {
    mappedGame.sides.visitor.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.parents[1]),
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
  return <BracketGame {...props} onClick={() => navigate(`/game/${props.game.gameId}`)} />;
};

export default TournamentBracket;
