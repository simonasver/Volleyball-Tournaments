export interface Game {
  id: string;
  ownerId: string;
  title: string;
  description: string;
  pointsToWin: number;
  pointDifferenceToWin: number;
  setsToWin: number;
  playersPerTeam: number;
  firstTeam: Team;
  secondTeam: Team;
  createDate: string;
  status: GameStatus;
  firstTeamScore: number;
  secondTeamScore: number;
  requestedTeams: Team[];
}

export enum GameStatus {
  New = 0,
  SingleTeam = 1,
  Ready = 2,
  Started = 3,
  Finished = 4
}

export interface Set {
  id: string;

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
