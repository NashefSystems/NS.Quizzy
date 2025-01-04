import { Injectable } from '@angular/core';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})
export abstract class EntityBaseService<PayloadDto, Dto> extends BaseService {

  get() {
    return this.httpClient.get<Dto[]>(this.getBaseUrl());
  }

  getById(id: string) {
    return this.httpClient.get<Dto>(`${this.getBaseUrl()}/${id}`);
  }

  insert(item: PayloadDto) {
    return this.httpClient.post<PayloadDto>(this.getBaseUrl(), item);
  }

  update(id: string, item: PayloadDto) {
    return this.httpClient.put<PayloadDto>(`${this.getBaseUrl()}/${id}`, item);
  }

  delete(id: string) {
    return this.httpClient.delete(`${this.getBaseUrl()}/${id}`);
  }
}
