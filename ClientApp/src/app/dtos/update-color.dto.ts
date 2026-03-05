import { TeamColor } from '../enums/team-color.enum';

export interface UpdateColorDto {
  q: number;
  r: number;
  hexColor: string;
  teamColor: TeamColor;
}
