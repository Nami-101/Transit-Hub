import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TrainSearchDto, TrainSearchResultDto } from '../models/train-search.dto';

@Injectable({
  providedIn: 'root'
})
export class TrainService {
  private readonly baseUrl = 'http://localhost:5000/api'; // Backend API URL

  constructor(private http: HttpClient) { }

  searchTrains(dto: TrainSearchDto): Observable<TrainSearchResultDto[]> {
    return this.http.post<TrainSearchResultDto[]>(`${this.baseUrl}/search/trains`, dto);
  }
}