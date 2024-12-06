import { AfterViewInit, Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { AppSettingsService } from '../../../services/app-settings.service';

@Component({
  selector: 'app-root',
  templateUrl: './root.component.html',
  styleUrl: './root.component.scss'
})
export class RootComponent implements AfterViewInit, OnInit {
  appContainerClasses = {
    "app-container": true,
    "large-screen": false
  };

  constructor(
    private readonly appSettingsService: AppSettingsService,
  ) {
    document.documentElement.style.setProperty("--app-max-width", appSettingsService.appMaxWidth + "px");
  }

  ngOnInit(): void {
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
