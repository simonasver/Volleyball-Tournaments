export interface Game {
  id: string;
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
}

export enum GameStatus {
  "New" = 0,
  "In progress" = 1,
  "Finished" = 2
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
