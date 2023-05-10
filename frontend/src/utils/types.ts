import { User } from "../store/auth-slice";

export interface Tournament {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  singleThirdPlace: boolean;
  basic: boolean;
  type: TournamentType;
  pointsToWin: number;
  pointsToWinLastSet: number;
  pointDifferenceToWin: number;
  maxTeams: number;
  maxSets: number;
  playersPerTeam: number;
  isPrivate: boolean;
  createDate: string;
  lastEditDate: string;
  status: TournamentStatus;
  requestedTeams: Team[];
  acceptedTeams: GameTeam[];
  winner: GameTeam;
  matches: TournamentMatch[];
  ownerId: string;
  managers: User[];
}

export interface TournamentMatch {
  id: string;
  round: number;
  thirdPlace: boolean;
  game?: Game;
  firstParentId: string;
  firstParent?: TournamentMatch;
  secondParentId: string;
  secondParent?: TournamentMatch;
  tournamentId: string;
  tournament: Tournament;
}

export enum TournamentType {
  SingleElimination = 0,
  DoubleElimination = 1,
  RoundRobin = 2
}

export enum TournamentStatus {
  Open = 0,
  Closed = 1,
  Started = 2,
  Finished = 3
}

export interface Game {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  basic: boolean;
  pointsToWin: number;
  pointsToWinLastSet: number;
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
  tournamentMatch: TournamentMatch;
  ownerId: string;
  managers: User[];
}

export enum GameScore {
  Score = 0,
  Kills = 1,
  Errors = 2,
  Attempts = 3,
  SuccessfulBlocks = 4,
  Blocks = 5,
  Touches = 6,
  BlockingErrors = 7,
  Aces = 8,
  ServingErrors = 9,
  TotalServes = 10,
  SuccessfulDigs = 11,
  BallTouches = 12,
  BallMisses = 13
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
  duplicate: boolean;
  players: GameTeamPlayer[];
}

export interface GameTeamPlayer {
  id: string;
  name: string;
}

export interface GameSet {
  id: string;
  number: number;
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
  kills: number,
  errors: number,
  attempts: number,
  successfulBlocks: number,
  blocks: number,
  touches: number,
  blockingErrors: number,
  aces: number,
  servingErrors: number,
  totalServes : number,
  successfulDigs : number,
  ballTouches : number,
  ballMisses : number
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
  ownerId: string;
  managers: User[];
}

export interface TeamPlayer {
  id: string;
  name: string;
}

export enum GamePreset {
  Regular = 0,
  Beach = 1,
  None = 2
}

export interface Log {
  id: string;
  isPrivate: boolean;
  message: string;
  ownerId: string;
  time: string;
  tournament: Tournament;
  game: Game;
}

export interface PageData {
  totalCount: number,
  pageSize: number;
  currentPage: number;
  totalPages: number;
  previousPageLink: string;
  nextPageLink: string;
}
