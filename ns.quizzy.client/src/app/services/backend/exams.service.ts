import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IExamDto, IExamPayloadDto } from '../../models/backend/exam.dto';

@Injectable({
  providedIn: 'root'
})
export class ExamsService extends EntityBaseService<IExamPayloadDto, IExamDto> {
  controllerName: string = 'Exams';
}
