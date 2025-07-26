import { TeamColor } from '../enums/team-color.enum';

export interface UserResponseDto {
  id: number;
  username: string;
  team: TeamColor;
  isAdmin: boolean;
}
