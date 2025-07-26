import { TeamColor } from '../enums/team-color.enum';

export interface UserModel {
  id: number;
  username: string;
  teamColor: TeamColor;
  isAdmin: boolean;
}
