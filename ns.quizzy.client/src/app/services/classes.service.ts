import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IClassDto } from '../models/backend/class.dto';

@Injectable({
  providedIn: 'root'
})
export class ClassesService {
  private readonly http = inject(HttpClient);
  readonly baseUrl = '/api/classes';

  getAll() {
    return this.http.get<IClassDto[]>(`${this.baseUrl}`);
  }

}
