import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IMoedDto, IMoedPayloadDto } from '../../models/backend/moed.dto';

@Injectable({
  providedIn: 'root'
})
export class MoedsService extends EntityBaseService<IMoedPayloadDto, IMoedDto> {
  controllerName: string = 'Moeds';
}
