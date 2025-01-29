import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { AppSettingsService } from '../../../services/app-settings.service';
import { AppTranslateService } from '../../../services/app-translate.service';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './root.component.html',
  styleUrl: './root.component.scss'
})
export class RootComponent implements AfterViewInit, OnInit {
  private readonly _appSettingsService = inject(AppSettingsService);
  private readonly _appTranslateService = inject(AppTranslateService);
  isReady = false;
  isLoading = false;
  appContainerClasses = {
    "app-container": true,
    "large-screen": false
  };

  ngOnInit(): void {
    document.documentElement.style.setProperty("--app-max-width", this._appSettingsService.appMaxWidth + "px");

    this._appSettingsService.onLoadingStatusChange.subscribe({
      next: (loadingStatus) => {
        this.isLoading = loadingStatus;
      }
    });

    this._appTranslateService.onDirectionChange.subscribe({
      next: (dir) => {
        document.documentElement.style.setProperty("--app-dir", dir);
        document.documentElement.setAttribute("dir", dir);
        this.isReady = !!dir;
      }
    });

    this.onResize();
  }

  ngAfterViewInit(): void {
    window.addEventListener('resize', this.onResize.bind(this));
  }

  onResize() {
    const isLargeScreen = window.innerWidth > this._appSettingsService.appMaxWidth;
    this._appSettingsService.setLargeScreen(isLargeScreen);
    this.appContainerClasses['large-screen'] = isLargeScreen;
    document.documentElement.style.setProperty("--is-large-screen", isLargeScreen ? 'true' : 'false');
  }
}
