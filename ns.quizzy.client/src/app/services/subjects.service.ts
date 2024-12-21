import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SubjectDto } from '../../models/backend-models/subject.dto';

@Injectable({
  providedIn: 'root'
})
export class SubjectsService {
  private readonly http = inject(HttpClient);
  readonly baseUrl = '/api/subjects';

  getAll() {
    return this.http.get<SubjectDto[]>(`${this.baseUrl}`);
  }

}
