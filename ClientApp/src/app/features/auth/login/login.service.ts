import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { UserResponseDto } from '../../../dtos/user-response.dto';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  constructor(private http: HttpClient) {}

  loginUser(user: LoginUserDto): Observable<UserResponseDto> {
    return this.http.post<UserResponseDto>(
      `${environment.apiUrl}/user/login`,
      user
    );
  }
}
