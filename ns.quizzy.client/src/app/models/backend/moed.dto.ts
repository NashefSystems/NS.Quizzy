import { IBaseEntityDto } from './base-entity.dto';

export interface IMoedPayloadDto {
    name: string;
    itemOrder: number;
}

export interface IMoedDto extends IMoedPayloadDto, IBaseEntityDto { }