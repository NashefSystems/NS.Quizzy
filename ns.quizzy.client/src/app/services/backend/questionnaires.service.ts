import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IQuestionnaireDto, IQuestionnairePayloadDto } from '../../models/backend/questionnaire.dto';

@Injectable({
  providedIn: 'root'
})
export class QuestionnairesService extends EntityBaseService<IQuestionnairePayloadDto, IQuestionnaireDto> {
  controllerName: string = 'Questionnaires';
}
