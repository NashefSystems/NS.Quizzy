import { IBaseEntityDto } from './base-entity.dto';

export interface IExamPayloadDto {
    startTime: string;
    questionnaireId: string;
    examTypeId: string;
    duration: string;
    durationWithExtra: string;
    gradeIds: string[];
    classIds: string[];
}

export interface IExamDto extends IExamPayloadDto, IBaseEntityDto { }