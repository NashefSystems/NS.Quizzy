import { IBaseEntityDto } from './base-entity.dto';

export interface IQuestionnairePayloadDto {
    code: number;
    name: string;
    subjectId: string;
    duration: string;
    durationWithExtra: string;
}

export interface IQuestionnaireDto extends IQuestionnairePayloadDto, IBaseEntityDto {
}