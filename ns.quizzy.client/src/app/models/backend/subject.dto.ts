import { IBaseEntityDto } from './base-entity.dto';

export interface ISubjectPayloadDto {
    name: string;
    itemOrder: number;
}

export interface ISubjectDto extends ISubjectPayloadDto, IBaseEntityDto { }