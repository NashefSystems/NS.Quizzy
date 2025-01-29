import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IGradeDto, IGradePayloadDto } from '../../models/backend/grade.dto';

@Injectable({
  providedIn: 'root'
})
export class GradesService extends EntityBaseService<IGradePayloadDto, IGradeDto> {
  controllerName: string = 'Grades';
}
