import { Box } from "@mui/material";
import { Bracket, BracketGame } from "react-tournament-bracket";
import { TournamentMatch } from "../../../utils/types";
import { Game } from "react-tournament-bracket/lib/components/model";

const game2 = {
  id: "2",
  name: "semi-finals",
  scheduled: Number(new Date()),
  sides: {
    home: {
      team: {
        id: "12",
        name: "First",
      },
      score: {
        score: 1,
      },
    },
    visitor: {
      team: {
        id: "13",
        name: "Second",
      },
      score: {
        score: 0,
      },
    },
  },
};
const game3 = {
  id: "3",
  name: "semi-finals",
  scheduled: Number(new Date()),
  sides: {
    home: {
      team: {
        id: "11",
        name: "Third",
      },
      score: {
        score: 1,
      },
    },
    visitor: {
      team: {
        id: "12",
        name: "First",
      },
      score: {
        score: 0,
      },
    },
  },
};
const game1 = {
  id: "1",
  name: "finals",
  scheduled: Number(new Date()),
  sides: {
    home: {
      team: {
        id: "10",
        name: "Fourth",
      },
      score: {
        score: 2,
      },
      seed: {
        displayName: "A1",
        rank: 1,
        sourceGame: game2,
        sourcePool: {},
      },
    },
    visitor: {
      score: {
        score: 3,
      },
      seed: {
        displayName: "",
        rank: 1,
        sourceGame: game3,
        sourcePool: {},
      },
    },
  },
};

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

  if (tournamentGame.FirstParent) {
    mappedGame.sides.home.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.FirstParent),
        tournamentGames
      ),
      sourcePool: {},
    };
  }
  if (tournamentGame.SecondParent) {
    mappedGame.sides.visitor.seed = {
      displayName: "",
      rank: 1,
      sourceGame: MapGameToParentGames(
        MapGameDataToFrontEnd(tournamentGame.SecondParent),
        tournamentGames
      ),
      sourcePool: {},
    };
  }
  return mappedGame;
};

const TournamentBracket = (props: TournamentBracketProps) => {
  const { tournamentGames } = props;

  // FORMAT GAMES AND DISPLAY TO FRONT
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

const TournamentBracketGameComponent = (props: any) => {
  return <BracketGame {...props} onClick={() => console.log(props.game.id)} />;
};

export default TournamentBracket;
