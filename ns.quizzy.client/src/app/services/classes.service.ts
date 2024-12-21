import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ClassDto } from '../../models/backend-models/class.dto';

@Injectable({
  providedIn: 'root'
})
export class ClassesService {
  private readonly http = inject(HttpClient);
  readonly baseUrl = '/api/classes';

  getAll() {
    return this.http.get<ClassDto[]>(`${this.baseUrl}`);
  }

}
