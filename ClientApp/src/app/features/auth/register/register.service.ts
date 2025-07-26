import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterUserDto } from '../../../dtos/register-user.dto';
import { UserResponseDto } from '../../../dtos/user-response.dto';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RegisterService {
  constructor(private http: HttpClient) {}

  registerUser(user: RegisterUserDto): Observable<UserResponseDto> {
    return this.http.post<UserResponseDto>(`${environment.apiUrl}/user`, user);
  }
}
