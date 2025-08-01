import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginUserDto } from '../../../dtos/login-user.dto';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginResponseDto } from '../../../dtos/login-response.dto';

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
}
