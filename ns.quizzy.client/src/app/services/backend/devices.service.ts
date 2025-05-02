import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { IDeviceDto, IDevicePayloadDto } from '../../models/backend/device.dto';

@Injectable({
  providedIn: 'root'
})
export class DevicesService extends BaseService {
  controllerName: string = 'Devices';

  constructor() {
    super();
  }

  updateInfoAsync(request: IDevicePayloadDto) {
    return this.httpClient.patch<IDeviceDto>(`${this.getBaseUrl()}`, request);
  }
}
