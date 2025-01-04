import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { ISubjectPayloadDto, ISubjectDto } from '../../models/backend/subject.dto';

@Injectable({
  providedIn: 'root'
})
export class SubjectsService extends EntityBaseService<ISubjectPayloadDto, ISubjectDto> {
  controllerName: string = 'Subjects';
}
