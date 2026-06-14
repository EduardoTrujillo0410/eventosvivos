import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Venue } from '../models/venue.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private http = inject(HttpClient);
  listar(): Observable<Venue[]> {
    return this.http.get<Venue[]>(`${environment.apiUrl}/venues`);
  }
}