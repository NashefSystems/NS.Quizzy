import { IBaseEntityDto } from './base-entity.dto';

export interface IExamPayloadDto {
    startTime: string;
    questionnaireId: string;
    examTypeId: string;
    moedId: string;
    duration: string;
    durationWithExtra: string;
    gradeIds?: string[];
    improvementGradeIds?: string[];
    classIds?: string[];
    improvementClassIds?: string[];
}

export interface IExamDto extends IExamPayloadDto, IBaseEntityDto { }

export interface IExamFilterRequest {
    fromTime: string;
    toTime: string;
    examTypeIds?: string[];
    moedIds?: string[];
    questionnaireIds?: string[];
    gradeIds?: string[];
    classIds?: string[];
    subjectIds?: string[];
}

