import { TeamColor } from '../enums/team-color.enum';

export interface LoginResponseDto {
  token: string;
  username: string;
  team: TeamColor;
  isAdmin: boolean;
}
