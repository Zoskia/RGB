import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TeamColor } from '../../enums/team-color.enum';

@Injectable({
  providedIn: 'root',
})
export class HexGridService {
  constructor(private http: HttpClient) {}

  generateHexGrid(): Observable<any> {
    return this.http.get(`${environment.apiUrl}/grid/ping`);
  }
}
