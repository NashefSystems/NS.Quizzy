import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IClassDto, IClassPayloadDto } from '../../models/backend/class.dto';

@Injectable({
  providedIn: 'root'
})
export class ClassesService extends EntityBaseService<IClassPayloadDto, IClassDto> {
  controllerName: string = 'Classes';
}
