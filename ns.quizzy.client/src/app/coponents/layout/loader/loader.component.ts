import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { LoadingStatusService } from '../../../services/loading-status.service';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.scss'
})
export class LoaderComponent implements OnInit {
  private readonly loadingStatusService = inject(LoadingStatusService);
  isLoading = false;

  ngOnInit(): void {
    this.loadingStatusService.getLoadingStatus().subscribe({
      next: (loadingStatus) => {
        this.isLoading = loadingStatus;
      }
    });
  }
}
