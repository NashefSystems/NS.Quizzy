import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IExamDto, IExamFilterRequest, IExamPayloadDto } from '../../models/backend/exam.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExamsService extends EntityBaseService<IExamPayloadDto, IExamDto> {
  controllerName: string = 'Exams';

  override get(filterCompletedExams: boolean = false): Observable<IExamDto[]> {
    let url = this.getBaseUrl();
    if (filterCompletedExams) {
      url += "?filterCompletedExams=true"
    }
    return this.httpClient.get<IExamDto[]>(url);
  }

  filter(request: IExamFilterRequest) {
    let url = `${this.getBaseUrl()}/filter`;
    return this.httpClient.post<IExamDto[]>(url, request);
  }

  setAsVisible(examId: string) {
    let url = `${this.getBaseUrl()}/SetAsVisible/${examId}`;
    return this.httpClient.patch<IExamDto>(url, null);
  }
}
