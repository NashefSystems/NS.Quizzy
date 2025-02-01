import { HttpInterceptorFn } from '@angular/common/http';
import { AppSettingsService } from '../services/app-settings.service';
import { inject, Injectable } from '@angular/core';
import { finalize } from 'rxjs';

export const LoadingInterceptor: HttpInterceptorFn = (req, next) => {
  const appSettingsService = inject(AppSettingsService);
  if (req.url.endsWith(".json")) {
    return next(req);
  }

  // Start loading
  console.log("start loading mode");
  appSettingsService.setLoadingStatus(true);
  return next(req).pipe(
    finalize(() => {
      // Stop loading when request completes
      console.log("stop loading mode");
      appSettingsService.setLoadingStatus(false);
    })
  );
};
