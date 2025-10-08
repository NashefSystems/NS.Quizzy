import { UserRoles } from '../../enums/user-roles.enum';
import { IBaseEntityDto } from './base-entity.dto';

export interface IUserPayloadDto {
    email: string;
    idNumber?: string;
    fullName: string;
    role: UserRoles;
    classId: string | null;
}

export interface IUserDto extends IUserPayloadDto, IBaseEntityDto { }
