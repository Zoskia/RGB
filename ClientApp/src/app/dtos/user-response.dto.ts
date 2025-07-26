import { TeamColor } from '../enums/team-color.enum';

export interface UserResponseDto {
  id: number;
  username: string;
  teamColor: TeamColor;
  isAdmin: boolean;
}
