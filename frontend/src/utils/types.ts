export interface Team {
  id: string;
  title: string;
  pictureUrl: string;
  description: string;
  creationDate: string;
  lastEditDate: string;
  players: TeamPlayer[];
}

export interface TeamPlayer {
  id: string;
  name: string;
}
