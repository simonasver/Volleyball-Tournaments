export interface Tournament {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  type: TournamentType;
  maxTeams: number;
  isPrivate: boolean;
  createDate: string;
  lastEditDate: string;
  status: TournamentStatus;
  requestedTeams: Team[];
  acceptedTeams: GameTeam[];
}

export interface TournamentGame {
  id: string;
  round: number;
  game: Game;
  FirstParent: TournamentGame;
  SecondParent: TournamentGame;
}

export enum TournamentType {
  SingleElimination = 0,
  DoubleElimination = 1,
  RoundRobin = 2
}

export enum TournamentStatus {
  New = 0,
  Started = 1,
  Finished = 2
}

export interface Game {
  id: string;
  title: string;
  description: string;
  pointsToWin: number;
  pointDifferenceToWin: number;
  maxSets: number;
  playersPerTeam: number;
  firstTeam: Team;
  secondTeam: Team;
  sets: GameSet[];
  firstTeamScore: number;
  secondTeamScore: number;
  isPrivate: boolean;
  createDate: string;
  lastEditDate: string;
  status: GameStatus;
  startDate: string;
  winner: GameTeam;
  finishDate: string;
  requestedTeams: Team[];
  blockedTeams: Team[];
  ownerId: string;
}

export enum GameStatus {
  New = 0,
  SingleTeam = 1,
  Ready = 2,
  Started = 3,
  Finished = 4
}

export interface GameTeam {
  id: string;
  title: string;
  profilePicture: string;
  description: string;
  players: GameTeamPlayer[];
}

export interface GameTeamPlayer {
  id: string;
  name: string;
}

export interface GameSet {
  id: string;
  firstTeam: GameTeam;
  secondTeam: GameTeam;
  players: SetPlayer[];
  firstTeamScore: number;
  secondTeamScore: number;
  status: GameStatus;
  startDate: string;
  winner: GameTeam;
  finishDate: string;
}

export interface SetPlayer {
  id: string;
  name: string;
  score: number;
  team: boolean;
}

export interface Team {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  createDate: string;
  lastEditDate: string;
  players: TeamPlayer[];
}

export interface TeamPlayer {
  id: string;
  name: string;
}
