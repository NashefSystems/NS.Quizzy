import { IBaseEntityDto } from './base-entity.dto';

export interface IUserPayloadDto {
    email: string;
    idNumber?: string;
    fullName: string;
    role: string;
    classId: string | null;
}

export interface IUserDto extends IUserPayloadDto, IBaseEntityDto { }