import { IBaseEntityDto } from './base-entity.dto';

export interface IClassPayloadDto {
    name: string;
    children: IClassDto[];
}

export interface IClassDto extends IClassPayloadDto, IBaseEntityDto { }