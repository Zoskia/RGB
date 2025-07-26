import { TeamColor } from '../enums/team-color.enum';

export interface RegisterUserDto {
  username: string;
  password: string;
  teamColor: TeamColor;
}
