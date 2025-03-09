import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IUserDto, IUserPayloadDto } from '../../models/backend/user.dto';

@Injectable({
  providedIn: 'root'
})
export class UsersService extends EntityBaseService<IUserPayloadDto, IUserDto> {
  controllerName: string = 'Users';
}
