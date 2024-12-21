import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { AppSettingsService } from '../../../services/app-settings.service';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrl: './root.component.scss'
})
export class RootComponent implements AfterViewInit, OnInit {
  private readonly appSettingsService = inject(AppSettingsService);
  appContainerClasses = {
    "app-container": true,
    "large-screen": false
  };

  ngOnInit(): void {
    document.documentElement.style.setProperty("--app-max-width", this.appSettingsService.appMaxWidth + "px");
    this.onResize();
  }

  ngAfterViewInit(): void {
    window.addEventListener('resize', this.onResize.bind(this));
  }

  onResize() {
    const viewportWidth = window.innerWidth;
    this.appContainerClasses['large-screen'] = viewportWidth > this.appSettingsService.appMaxWidth;
  }
}
