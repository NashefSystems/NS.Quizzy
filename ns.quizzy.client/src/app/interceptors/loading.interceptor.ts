import { HttpInterceptorFn } from '@angular/common/http';
import { AppSettingsService } from '../services/app-settings.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

export const LoadingInterceptor: HttpInterceptorFn = (req, next) => {
  const appSettingsService = inject(AppSettingsService);
  if (req.url.endsWith(".json")) {
    return next(req);
  }

  appSettingsService.setLoadingStatus(true);
  return next(req).pipe(
    finalize(() => {
      appSettingsService.setLoadingStatus(false);
    })
  );
};
