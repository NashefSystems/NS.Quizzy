import { IBaseEntityDto } from './base-entity.dto';

export interface IClassPayloadDto {
    gradeId: string;
    name: string;
    code: number;
}

export interface IClassDto extends IClassPayloadDto, IBaseEntityDto {
    fullCode: number;
    children: IClassDto[];
}