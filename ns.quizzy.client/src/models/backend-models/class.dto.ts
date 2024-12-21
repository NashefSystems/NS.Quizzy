import { BaseEntityDto } from "./base-entity.dto";

export class ClassDto extends BaseEntityDto {
    name: string
    children: ClassDto[];
}