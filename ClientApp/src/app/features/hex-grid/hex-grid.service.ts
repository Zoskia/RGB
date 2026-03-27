import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CellModel } from '../../models/cell.model';
import { UpdateColorDto } from '../../dtos/update-color.dto';

@Injectable({
  providedIn: 'root',
})
export class HexGridService {
  constructor(private http: HttpClient) {}

  getGrid(team: number): Observable<CellModel[]> {
    return this.http.get<CellModel[]>(`${environment.apiUrl}/grid/${team}`);
  }

  updateCellColor(cell: UpdateColorDto): Observable<void> {
    localStorage.getItem;
    return this.http.put<void>(`${environment.apiUrl}/grid/cell`, cell);
  }
}
