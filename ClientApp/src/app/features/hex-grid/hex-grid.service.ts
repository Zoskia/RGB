import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CellModel } from '../../models/cell.model';

@Injectable({
  providedIn: 'root',
})
export class HexGridService {
  constructor(private http: HttpClient) {}

  // generateHexGrid(): Observable<any> {
  //   return this.http.get(`${environment.apiUrl}/grid/ping`);
  // }

  getGrid(team: number): Observable<CellModel[]> {
    return this.http.get<CellModel[]>(`${environment.apiUrl}/grid/${team}`);
  }
}
