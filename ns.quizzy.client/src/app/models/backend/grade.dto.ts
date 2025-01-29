import { IBaseEntityDto } from './base-entity.dto';
import { IClassDto } from './class.dto';

export interface IGradePayloadDto {
    name: string;
    code: number;
}

export interface IGradeDto extends IGradePayloadDto, IBaseEntityDto {
    classes: IClassDto[];
}