import { IBaseEntityDto } from './base-entity.dto';

export interface IExamTypePayloadDto {
    name: string;
    itemOrder: number;
}

export interface IExamTypeDto extends IExamTypePayloadDto, IBaseEntityDto { }