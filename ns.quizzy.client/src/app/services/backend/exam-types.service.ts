import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IExamTypeDto, IExamTypePayloadDto } from '../../models/backend/exam-type.dto';

@Injectable({
  providedIn: 'root'
})
export class ExamTypesService extends EntityBaseService<IExamTypePayloadDto, IExamTypeDto> {
  controllerName: string = 'ExamTypes';
}
