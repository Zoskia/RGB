import { TeamColor } from '../enums/team-color.enum';

export interface CellModel {
  q: number;
  r: number;
  hexColor: string;
  teamColor: TeamColor;
}
