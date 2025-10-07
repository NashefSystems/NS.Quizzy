import { Injectable } from '@angular/core';
import { EntityBaseService } from './entity-base.service';
import { IUserDto, IUserPayloadDto } from '../../models/backend/user.dto';
import { IUploadFileStatusResponse } from '../../models/backend/upload-file-status.response';
import { IUploadFileResponse } from '../../models/backend/upload-file.response';
import { IUserLoginStatusDto } from '../../models/backend/user-login-status.dto';

@Injectable({
  providedIn: 'root'
})
export class UsersService extends EntityBaseService<IUserPayloadDto, IUserDto> {
  controllerName: string = 'Users';

  upload(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.httpClient.post<IUploadFileResponse>(`${this.getBaseUrl()}/Upload`, formData);
  }

  uploadFileStatus(uploadMessageId: string) {
    return this.httpClient.get<IUploadFileStatusResponse>(`${this.getBaseUrl()}/UploadFileStatus/${uploadMessageId}`);
  }

  getUsersLoginStatus() {
    return this.httpClient.get<IUserLoginStatusDto[]>(`${this.getBaseUrl()}/UsersLoginStatus`);
  }
}
