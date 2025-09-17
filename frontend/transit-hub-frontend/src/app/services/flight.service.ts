import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FlightSearchDto, FlightSearchResultDto } from '../models/flight-search.dto';

@Injectable({
  providedIn: 'root'
})
export class FlightService {
  private readonly baseUrl = 'http://localhost:5000/api'; // Backend API URL

  constructor(private http: HttpClient) { }

  searchFlights(dto: FlightSearchDto): Observable<FlightSearchResultDto[]> {
    return this.http.post<FlightSearchResultDto[]>(`${this.baseUrl}/search/flights`, dto);
  }
}