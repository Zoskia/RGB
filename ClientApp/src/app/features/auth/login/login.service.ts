import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginResponseDto } from '../../../dtos/login-response.dto';

const STORAGE_KEYS = {
  token: 'token',
  username: 'username',
  team: 'team',
  isAdmin: 'isAdmin',
} as const;

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  constructor(private http: HttpClient) {}

  loginUser(user: LoginUserDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(
      `${environment.apiUrl}/user/login`,
      user
    );
  }

  storeSession(response: LoginResponseDto): void {
    localStorage.setItem(STORAGE_KEYS.token, response.token);
    localStorage.setItem(STORAGE_KEYS.username, response.username);
    localStorage.setItem(STORAGE_KEYS.team, response.team.toString());
    localStorage.setItem(STORAGE_KEYS.isAdmin, response.isAdmin.toString());
  }

  getToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.token);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEYS.token);
    localStorage.removeItem(STORAGE_KEYS.username);
    localStorage.removeItem(STORAGE_KEYS.team);
    localStorage.removeItem(STORAGE_KEYS.isAdmin);
  }
}
